using FBZ_System.Repositories;
using FBZ_System.Services;
using FBZ_System.Strategies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Services.Persistence;
using Microsoft.AspNetCore.Authentication;
using FBZSystemMvc.Services.DatasetUpdates;
using FBZSystemMvc.Services.ExternalApis;


var builder = WebApplication.CreateBuilder(args);

// MVC + Razor Pages (Identity UI uses Razor Pages)
builder.Services.AddControllersWithViews()
    .AddRazorOptions(o =>
    {
        o.ViewLocationFormats.Add("/Views/Staff/{1}/{0}.cshtml");
    });
builder.Services.AddRazorPages();

// SQLite + EF Core + Identity (with Roles)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>(); 

// FIX LOGIN REDIRECT PATH
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Your existing app services
builder.Services.AddSingleton<IComicRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data");
    return new ReloadableComicRepository(dataPath);
});

// expose the same singleton as IDatasetReloadable
builder.Services.AddSingleton<IDatasetReloadable>(sp =>
    (IDatasetReloadable)sp.GetRequiredService<IComicRepository>());

builder.Services.AddScoped<FBZSystemMvc.Services.Persistence.IAnalyticsService, FBZSystemMvc.Services.Persistence.AnalyticsService>();
builder.Services.AddScoped<FBZSystemMvc.Services.Persistence.ISavedComicsService, FBZSystemMvc.Services.Persistence.SavedComicsService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

builder.Services.AddSingleton<IGroupingStrategy, GroupByAuthorStrategy>();
builder.Services.AddSingleton<IGroupingStrategy, GroupByYearStrategy>();

builder.Services.AddSingleton<ISortStrategy, SortTitleAscendingStrategy>();
builder.Services.AddSingleton<ISortStrategy, SortTitleDescendingStrategy>();

builder.Services.AddSingleton<ISearchService, SearchService>();
builder.Services.AddSingleton<ComicFormatter>();
builder.Services.AddSingleton<ISearchHistoryService, SearchHistoryService>();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<FBZSystemMvc.Services.SearchListStore>();

builder.Services.AddHttpClient("dataset");
builder.Services.AddHttpClient<IGoogleBooksService, GoogleBooksService>();
builder.Services.AddHttpClient<IOpenLibraryService, OpenLibraryService>();

builder.Services.Configure<DatasetUpdateOptions>(builder.Configuration.GetSection("DatasetUpdate"));
builder.Services.AddSingleton<IDatasetUpdateService, DatasetUpdateService>();
builder.Services.AddHostedService<DatasetUpdateHostedService>();

builder.Services.AddHttpClient<IComicVineService, ComicVineService>();
builder.Services.AddHttpClient<IWikipediaService, WikipediaService>();

var app = builder.Build();


// Only override the URL when EB sets PORT; locally launchSettings.json controls the port.
var ebPort = Environment.GetEnvironmentVariable("PORT");
if (ebPort is not null)
    app.Urls.Add($"http://0.0.0.0:{ebPort}");

// ✅ APPLY MIGRATIONS + SEED STAFF ROLE
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Staff"))
    {
        await roleManager.CreateAsync(new IdentityRole("Staff"));
    }
}


// Middleware order
// Note: UseHttpsRedirection is intentionally omitted — EB terminates TLS at
// the load balancer and forwards plain HTTP to the app. Redirecting to HTTPS
// here would cause an infinite redirect loop and a 502.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.Use(async (ctx, next) =>
{
    if (HttpMethods.IsGet(ctx.Request.Method) &&
        ctx.Request.Query.TryGetValue("area", out var area) && area == "Identity" &&
        ctx.Request.Query.TryGetValue("page", out var page) && page == "/Account/Logout")
    {
        await ctx.SignOutAsync(IdentityConstants.ApplicationScheme);

        var returnUrl = ctx.Request.Query.TryGetValue("returnUrl", out var r) ? r.ToString() : "/";
        ctx.Response.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
        return;
    }

    await next();
});

app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dataset}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { } // for integration testing access to the Program class