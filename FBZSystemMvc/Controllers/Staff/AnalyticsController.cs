using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBZSystemMvc.Models.Staff;
using FBZSystemMvc.Services.Persistence;

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
    public async Task<IActionResult> Index()
    {
        var recent = await _analytics.GetRecentAsync(250);
        var top = await _analytics.GetTopQueriesAsync(25);

        return View(new AnalyticsDashboardViewModel
        {
            Recent = recent.ToList(),
            TopQueries = top.ToList()
        });
    }
}
