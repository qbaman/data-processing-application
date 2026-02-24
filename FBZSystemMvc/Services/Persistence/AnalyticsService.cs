using System;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using FBZ_System.Domain;
using FBZSystemMvc.Data;
using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Services.Persistence;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _db;

    public AnalyticsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task RecordSearchAsync(SearchQuery query, int resultCount, string? userId)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));

        var json = JsonSerializer.Serialize(new
        {
            query.TitleContains,
            query.AuthorContains,
            query.Genre,
            query.YearFrom,
            query.YearTo,
            query.Language,
            query.Edition,
            query.NameType,
            query.ResourceType,
            query.Topics,
            query.PhysicalDescription,
            query.ContentType,
            query.SortDescending,
            query.GroupBy
        });

        _db.SearchAnalytics.Add(new SearchAnalyticsEvent
        {
            UserId = string.IsNullOrWhiteSpace(userId) ? null : userId,
            QueryJson = json,
            ResultCount = resultCount,
            SearchedAtUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<SearchAnalyticsEvent>> GetRecentAsync(int take = 250)
    {
        return await _db.SearchAnalytics
            .AsNoTracking()
            .OrderByDescending(x => x.SearchedAtUtc)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<(string QueryJson, int Count)>> GetTopQueriesAsync(int take = 25)
    {
        return await _db.SearchAnalytics
            .AsNoTracking()
            .GroupBy(x => x.QueryJson)
            .Select(g => new { g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => ValueTuple.Create(x.Key, x.Count))
            .ToListAsync();
    }
}
