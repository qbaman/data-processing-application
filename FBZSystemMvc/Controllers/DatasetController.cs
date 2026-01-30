using Microsoft.AspNetCore.Mvc;
using FBZ_System.Domain;
using FBZ_System.Services;
using FBZSystemMvc.Models;

namespace FBZSystemMvc.Controllers;

public class DatasetController : Controller
{
    private readonly ISearchService _search;

    public DatasetController(ISearchService search)
    {
        _search = search;
    }

    private readonly SearchListStore _listStore;

    public DatasetController(ISearchService search, SearchListStore listStore)
    {
        _search = search;
        _listStore = listStore;
    }

    [HttpPost]
    public IActionResult AddToList([FromQuery] SearchQuery query, int id)
    {
        var ids = _listStore.GetIds(HttpContext);
        if (!ids.Contains(id)) ids.Add(id);
        _listStore.SaveIds(HttpContext, ids);
        return RedirectToAction("Index", query);
    }

    [HttpPost]
    public IActionResult RemoveFromList([FromQuery] SearchQuery query, int id)
    {
        var ids = _listStore.GetIds(HttpContext);
        ids.Remove(id);
        _listStore.SaveIds(HttpContext, ids);
        return RedirectToAction("Index", query);
    }

    [HttpPost]
    public IActionResult ClearList([FromQuery] SearchQuery query)
    {
        _listStore.SaveIds(HttpContext, new List<int>());
        return RedirectToAction("Index", query);
    }



    [HttpGet]
    public IActionResult Index([FromQuery] SearchQuery query)
    {
        query ??= new SearchQuery();

        var result = _search.AdvancedSearch(query);

        var vm = new DatasetIndexViewModel
        {
            Query = result.Query,
            Results = result.Comics
        };

        return View(vm);
    }
}
