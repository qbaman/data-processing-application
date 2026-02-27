using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FBZ_System.Domain;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Services.Persistence;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _db;

    public AnalyticsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task RecordSearchAsync(string queryText, int resultCount, string? userId, IEnumerable<string>? resultComicIds = null)
    {
        var ev = new SearchAnalyticsEvent
        {
            UserId = userId,
            QueryText = queryText ?? string.Empty,
            ResultCount = resultCount,
            CreatedAtUtc = DateTime.UtcNow,
            ResultHits = (resultComicIds ?? Enumerable.Empty<string>())
                .Distinct()
                .Select(id => new SearchResultHit { ComicId = id })
                .ToList()
        };

        _db.SearchAnalyticsEvents.Add(ev);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<SearchAnalyticsEvent>> GetRecentAsync(int take = 250)
    {
        return await _db.SearchAnalyticsEvents
            .AsNoTracking()
            .OrderByDescending(x => x.QueryText)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<(string QueryText, int Count)>> GetTopQueriesAsync(int take = 25)
    {
        return await _db.SearchAnalyticsEvents
            .AsNoTracking()
            .GroupBy(x => x.QueryText)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => ValueTuple.Create(x.Key, x.Count))
            .ToListAsync();
    }
}
