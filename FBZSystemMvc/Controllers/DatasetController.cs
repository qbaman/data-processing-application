using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FBZ_System.Domain;
using FBZ_System.Services;
using FBZ_System.Repositories;
using FBZSystemMvc.Models;
using FBZSystemMvc.Services;
using Microsoft.EntityFrameworkCore;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Services.Persistence;
using Microsoft.AspNetCore.Routing;
using FBZSystemMvc.Services.ExternalApis;

namespace FBZSystemMvc.Controllers;

public class DatasetController : Controller
{
    private readonly ISearchService _search;
    private readonly ApplicationDbContext _db;
    private readonly IComicRepository _repo;
    private readonly SearchListStore _list;
    private readonly ComicFormatter _formatter;
    private readonly IAnalyticsService _analytics;
    private readonly IGoogleBooksService _googleBooks;
    private readonly IOpenLibraryService _openLibrary;
    private readonly IWikipediaService _wikipedia;

    public DatasetController(
        ISearchService search,
        IComicRepository repo,
        SearchListStore list,
        ComicFormatter formatter,
        ApplicationDbContext db,
        IAnalyticsService analytics,
        IGoogleBooksService googleBooks,
        IOpenLibraryService openLibrary,
        IWikipediaService wikipedia)
    {
        _search = search;
        _repo = repo;
        _list = list;
        _formatter = formatter;
        _db = db;
        _analytics = analytics;
        _googleBooks = googleBooks;
        _openLibrary = openLibrary;
        _wikipedia = wikipedia;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] SearchQuery query)
    {
        query ??= new SearchQuery();

        var result = _search.AdvancedSearch(query);

        _analytics.RecordSearch(query, result.Comics, User);

        var ids = _list.GetIds(HttpContext);
        var all = _repo.GetAllComics().ToList();

        var listComics = ids
            .Select(id => all.FirstOrDefault(c => c.Id == id))
            .Where(c => c is not null)
            .Cast<Comic>()
            .ToList();

        // Ppaaging
        var page = result.Query.Page <= 0 ? 1 : result.Query.Page;
        var pageSize = result.Query.PageSize <= 0 ? 50 : result.Query.PageSize;

        var total = result.Comics.Count;
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        if (totalPages < 1) totalPages = 1;
        if (page > totalPages) page = totalPages;

        var paged = result.Comics
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        result.Query.Page = page;
        result.Query.PageSize = pageSize;

        var vm = new DatasetIndexViewModel
        {
            Query = result.Query,
            Results = paged,
            TotalResults = total,
            TotalPages = totalPages,
            SearchList = listComics,
            AllPhysicalDescriptions = _repo.GetAllPhysicalDescriptions().ToList(),
            AllGenres = _repo.GetAllGenres().ToList(),
            AllLanguages = _repo.GetAllLanguages().ToList(),
            AllEditions = _repo.GetAllEditions().ToList(),
            AllResourceTypes = _repo.GetAllResourceTypes().ToList(),
            AllTopics = _repo.GetAllTopics().ToList(),
            AllContentTypes = _repo.GetAllContentTypes().ToList(),
        };

        // grouping
        vm.GroupedResults = _search.GroupResults(result.Query, paged);

        vm.AllNameTypes = _repo.GetAllNameTypes().ToList();

        return View(vm);
    }

    [HttpPost]
    public IActionResult AddToList(SearchQuery query, string id, string? kiosk)
    {
        _list.Add(HttpContext, id);

        var rv = new RouteValueDictionary(query);
        if (kiosk == "1") rv["kiosk"] = "1";

        return RedirectToAction(nameof(Index), rv);
    }

    [HttpGet]
    public IActionResult ExportList()
    {
        var ids = _list.GetIds(HttpContext);
        var all = _repo.GetAllComics().ToList();

        var comics = ids
            .Select(id => all.FirstOrDefault(c => c.Id == id))
            .Where(c => c != null)
            .Cast<Comic>()
            .ToList();

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Title,Authors,Years,Genres,ISBNs");

        foreach (var c in comics)
        {
            var authors = string.Join(" | ", c.Authors);
            var years = string.Join(" | ", c.Years);
            var genres = string.Join(" | ", c.Genres);
            var isbns = string.Join(" | ", c.Isbns);

            sb.AppendLine($"\"{c.Id}\",\"{c.MainTitle}\",\"{authors}\",\"{years}\",\"{genres}\",\"{isbns}\"");
        }

        return File(
            System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
            "text/csv",
            "search-list.csv"
        );
    }

    [HttpPost]
    public IActionResult RemoveFromList(SearchQuery query, string id, string? kiosk)
    {
        _list.Remove(HttpContext, id);

        var rv = new RouteValueDictionary(query);
        if (kiosk == "1") rv["kiosk"] = "1";

        return RedirectToAction(nameof(Index), rv);
    }


    [HttpPost]
    public IActionResult ClearList(SearchQuery query, string? kiosk)
    {
        _list.Clear(HttpContext);

        var rv = new RouteValueDictionary(query);
        if (kiosk == "1") rv["kiosk"] = "1";

        return RedirectToAction(nameof(Index), rv);
    }

    public IActionResult Finish()
    {
        _list.Clear(HttpContext);           // clears the session search list
        return RedirectToAction("Index", "Home");
    }

[HttpGet]
public async Task<IActionResult> Details(string id, CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(id))
        return RedirectToAction(nameof(Index));

    var comic = _repo.GetAllComics().FirstOrDefault(c => c.Id == id);
    if (comic is null)
        return RedirectToAction(nameof(Index));

    var vm = new ComicDetailsViewModel
    {
        Comic = comic,
        AuthorsDisplay = comic.Authors?.Distinct().OrderBy(a => a).ToList() ?? new(),
        GenresDisplay = comic.Genres?.Distinct().OrderBy(g => g).ToList() ?? new(),
        YearsDisplay = comic.Years?.Distinct().OrderBy(y => y).ToList() ?? new List<int>(),
        IsbnsDisplay = (comic.Isbns ?? new()).Select(i => string.IsNullOrWhiteSpace(i) ? "missing" : i).Distinct().ToList()
    };

    vm.InfoLines = _formatter.BuildInfoLines(comic);

    var flag = _db.FlaggedComics.AsNoTracking().FirstOrDefault(f => f.ComicId == id);
    vm.IsFlagged = flag != null;
    vm.FlagReason = flag?.Reason;

    vm.GoogleBooks = await _googleBooks.LookupAsync(comic, cancellationToken);
    vm.OpenLibrary = await _openLibrary.LookupAsync(comic, cancellationToken);

    var firstAuthor = comic.Authors?.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a));
    if (firstAuthor is not null)
        vm.Wikipedia = await _wikipedia.LookupAuthorAsync(firstAuthor, cancellationToken);

    var firstValidIsbn = vm.IsbnsDisplay
        .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i) && i.Trim().ToLower() != "missing");

    if (!string.IsNullOrWhiteSpace(firstValidIsbn))
    {
        vm.OpenLibraryCoverUrl =
            $"https://covers.openlibrary.org/b/isbn/{Uri.EscapeDataString(firstValidIsbn)}-L.jpg?default=false";
    }

        return View(vm);
    }

    public IActionResult Exit()
    {
        _list.Clear(HttpContext);
        return RedirectToAction("Index", "Home");
    }
}