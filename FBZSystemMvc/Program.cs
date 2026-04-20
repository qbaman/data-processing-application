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
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews()
    .AddRazorOptions(o =>
    {
        o.ViewLocationFormats.Add("/Views/Staff/{1}/{0}.cshtml");
    });
builder.Services.AddRazorPages();

// database + identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// repositories
builder.Services.AddSingleton<IComicRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data");
    return new ReloadableComicRepository(dataPath);
});

builder.Services.AddSingleton<IDatasetReloadable>(sp =>
    (IDatasetReloadable)sp.GetRequiredService<IComicRepository>());

// services
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

// rate limiting — 30 requests/min per IP on /Dataset
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("dataset", limiter =>
    {
        limiter.PermitLimit = 30;
        limiter.Window = TimeSpan.FromMinutes(1);
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiter.QueueLimit = 0;
    });
    options.RejectionStatusCode = 429;
});

builder.Services.AddSingleton<FBZSystemMvc.Services.SearchListStore>();
builder.Services.AddSingleton<FBZSystemMvc.Services.RecentlyViewedStore>();

// external API clients
builder.Services.AddHttpClient("dataset");
builder.Services.AddHttpClient<IGoogleBooksService, GoogleBooksService>();
builder.Services.AddHttpClient<IOpenLibraryService, OpenLibraryService>();
builder.Services.AddHttpClient<IComicVineService, ComicVineService>();
builder.Services.AddHttpClient<IWikipediaService, WikipediaService>();
builder.Services.AddHttpClient<IEmailService, EmailService>();

// dataset update
builder.Services.Configure<DatasetUpdateOptions>(builder.Configuration.GetSection("DatasetUpdate"));
builder.Services.AddSingleton<IDatasetUpdateService, DatasetUpdateService>();
builder.Services.AddHostedService<DatasetUpdateHostedService>();

var app = builder.Build();

// EB port binding
var ebPort = Environment.GetEnvironmentVariable("PORT");
if (ebPort is not null)
    app.Urls.Add($"http://0.0.0.0:{ebPort}");

// migrations + seed Staff role
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();

// Security headers on every response
app.Use(async (ctx, next) =>
{
    var h = ctx.Response.Headers;
    h["X-Content-Type-Options"]  = "nosniff";
    h["X-Frame-Options"]         = "SAMEORIGIN";
    h["Referrer-Policy"]         = "strict-origin-when-cross-origin";
    h["Permissions-Policy"]      = "camera=(), microphone=(), geolocation=()";
    h["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline'; " +
        "style-src 'self' 'unsafe-inline' cdn.jsdelivr.net; " +
        "font-src 'self' cdn.jsdelivr.net; " +
        "img-src 'self' data: api.qrserver.com covers.openlibrary.org " +
            "upload.wikimedia.org books.google.com; " +
        "connect-src 'self'; " +
        "frame-ancestors 'none';";
    await next();
});

app.UseRouting();
app.UseRateLimiter();
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

app.MapControllerRoute(
    name: "dataset",
    pattern: "Dataset/{action=Index}/{id?}",
    defaults: new { controller = "Dataset" })
    .RequireRateLimiting("dataset");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dataset}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();

public partial class Program { } // for integration tests
