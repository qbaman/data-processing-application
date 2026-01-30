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

    public DatasetController(ISearchService search, IComicRepository repo, SearchListStore list)
    {
        _search = search;
        _repo = repo;
        _list = list;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] SearchQuery query)
    {
        query ??= new SearchQuery();

        // Run the search (full result set)
        var result = _search.AdvancedSearch(query);

        // Search List (session)
        var ids = _list.GetIds(HttpContext);
        var all = _repo.GetAllComics();

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
        if (totalPages == 0) totalPages = 1;
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
            SearchList = listComics
        };

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
}
