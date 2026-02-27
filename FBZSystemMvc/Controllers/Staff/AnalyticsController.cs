using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBZSystemMvc.Models.Staff;
using FBZSystemMvc.Services.Persistence;
using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Controllers.Staff;

[Authorize(Roles = "Staff")]
[Route("staff/analytics")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analytics;

    public AnalyticsController(IAnalyticsService analytics)
    {
        _analytics = analytics;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        // Temporary "build-green" dashboard. We'll wire real queries once DB/migrations are stable.
        var vm = new AnalyticsDashboardViewModel
        {
            RecentSearches = new List<SearchAnalyticsEvent>(),
            TopQueries = new List<(string Query, int Count)>(),
            TopResults = new List<(string ComicId, int Count)>(),
            ComicsOver100 = new List<(string ComicId, int Count)>()
        };

        return View(vm);
    }
}