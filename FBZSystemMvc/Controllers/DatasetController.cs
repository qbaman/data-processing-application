using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using FBZ_System.Domain;
using FBZ_System.Services;
using FBZ_System.Repositories;
using FBZSystemMvc.Models;
using FBZSystemMvc.Services;


namespace FBZSystemMvc.Controllers;

public class DatasetController : Controller
{
    private readonly ISearchService _search;
    private readonly IComicRepository _repo;
    private readonly SearchListStore _list;
    private readonly ComicFormatter _formatter;

    public DatasetController(ISearchService search, IComicRepository repo, SearchListStore list, ComicFormatter formatter)
    {
        _search = search;
        _repo = repo;
        _list = list;
        _formatter = formatter;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] SearchQuery query)
    {
        query ??= new SearchQuery();

        // Run the search (full result set)
        var result = _search.AdvancedSearch(query);

        // Session search list
        var ids = _list.GetIds(HttpContext);
        var all = _repo.GetAllComics().ToList();

        var listComics = ids
            .Select(id => all.FirstOrDefault(c => c.Id == id))
            .Where(c => c is not null)
            .Cast<Comic>()
            .ToList();

        // Paging
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
            AllGenres = _repo.GetAllGenres().ToList()
        };

        // Grouping (uses the *paged* results here)
        vm.GroupedResults = _search.GroupResults(result.Query, paged);

        return View(vm);
    }

    [HttpPost]
    public IActionResult AddToList(SearchQuery query, string id)
    {
        _list.Add(HttpContext, id);
        return RedirectToAction(nameof(Index), query);
    }

    [HttpPost]
    public IActionResult RemoveFromList(SearchQuery query, string id)
    {
        _list.Remove(HttpContext, id);
        return RedirectToAction(nameof(Index), query);
    }

    [HttpPost]
    public IActionResult ClearList(SearchQuery query)
    {
        _list.Clear(HttpContext);
        return RedirectToAction(nameof(Index), query);
    }

    [HttpGet]

    [HttpGet]
    public IActionResult Details(string id)
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
            YearsDisplay = comic.Years?.Distinct().OrderBy(y => y).ToList() ?? new(),
            IsbnsDisplay = (comic.Isbns ?? new()).Select(i => string.IsNullOrWhiteSpace(i) ? "missing" : i).Distinct().ToList()
        };

        vm.InfoLines = _formatter.BuildInfoLines(comic);

        vm.InfoLines = comic.ExtraAttributes
            .OrderBy(kvp => kvp.Key)
            .SelectMany(kvp => (kvp.Value ?? new List<string>()).Select(v => $"{kvp.Key}: {v}"))
            .ToList();


        return View(vm);
    }


    public IActionResult Exit()
    {
        _list.Clear(HttpContext);
        return RedirectToAction("Index", "Home");
    }
}