using FBZ_System.Repositories;
using FBZ_System.Services;
using FBZ_System.Strategies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IComicRepository>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var dataPath = Path.Combine(env.ContentRootPath, "Data");
    return new ComicRepositoryCsv(dataPath);
});

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
app.UseRouting();
app.UseSession();
app.UseAuthorization();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dataset}/{action=Index}/{id?}");


app.Run();
