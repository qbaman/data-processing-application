using FBZ_System.Repositories;
using FBZ_System.Services;
using FBZ_System.Strategies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FBZSystemMvc.Persistence;

var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Pages (Identity UI uses Razor Pages)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// SQLite + EF Core + Identity (with Roles)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Your existing app services
builder.Services.AddSingleton<IComicRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data");
    return new ComicRepositoryCsv(dataPath);
});

builder.Services.AddScoped<FBZSystemMvc.Services.Persistence.IAnalyticsService, FBZSystemMvc.Services.Persistence.AnalyticsService>();
builder.Services.AddScoped<FBZSystemMvc.Services.Persistence.ISavedComicsService, FBZSystemMvc.Services.Persistence.SavedComicsService>();

builder.Services.AddSingleton<IGroupingStrategy, GroupByAuthorStrategy>();
builder.Services.AddSingleton<IGroupingStrategy, GroupByYearStrategy>();

builder.Services.AddSingleton<ISortStrategy, SortTitleAscendingStrategy>();
builder.Services.AddSingleton<ISortStrategy, SortTitleDescendingStrategy>();

builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<ComicFormatter>();
builder.Services.AddSingleton<ISearchHistoryService, SearchHistoryService>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<FBZSystemMvc.Services.SearchListStore>();

var app = builder.Build();

// Ensure DB exists + apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Middleware order
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dataset}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
