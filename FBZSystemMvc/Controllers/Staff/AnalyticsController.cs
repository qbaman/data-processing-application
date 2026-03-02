using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBZSystemMvc.Models.Staff;
using FBZSystemMvc.Services.Persistence;
using FBZ_System.Repositories;

namespace FBZSystemMvc.Controllers.Staff;

[Authorize(Roles = "Staff")]
[Route("staff/analytics")]
public class AnalyticsController : Controller
{
    private readonly IAnalyticsService _analytics;
    private readonly IComicRepository _repo;

    public AnalyticsController(IAnalyticsService analytics, IComicRepository repo)
    {
        _analytics = analytics;
        _repo = repo;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var titleById = _repo.GetAllComics()
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => g.First().MainTitle);

        var vm = new AnalyticsDashboardViewModel
        {
            RecentSearches = _analytics.GetRecentSearches(20)
                .Select(e => new RecentSearchRow
                {
                    OccurredUtc = e.OccurredUtc,
                    QuerySignature = e.QuerySignature,
                    TotalResults = e.TotalResults,
                    CountedResults = e.CountedResults,
                    Truncated = e.Truncated
                })
                .ToList(),

            TopQueries = _analytics.GetTopQueries(10)
                .Select(x => new TopQueryRow { Query = x.Query, Count = x.Count })
                .ToList(),

            TopResults = _analytics.GetTopResults(10)
                .Select(x => new TopResultRow
                {
                    ComicId = x.ComicId,
                    Title = titleById.TryGetValue(x.ComicId, out var t) ? t : x.ComicId,
                    Count = x.Count
                })
                .ToList(),

            ComicsOver100 = _analytics.GetComicsOverThreshold(100, 200)
                .Select(x => new OverThresholdRow
                {
                    ComicId = x.ComicId,
                    Title = titleById.TryGetValue(x.ComicId, out var t) ? t : x.ComicId,
                    Count = x.Count
                })
                .ToList()
        };

        return View(vm);
    }
}