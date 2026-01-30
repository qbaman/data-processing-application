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
