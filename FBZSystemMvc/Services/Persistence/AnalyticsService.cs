using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using FBZ_System.Domain;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Services.Persistence;

public class AnalyticsService : IAnalyticsService
{
    private readonly ApplicationDbContext _db;

    private const int MaxComicsToCountPerSearch = 200;

    public AnalyticsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public void RecordSearch(SearchQuery query, IReadOnlyList<Comic> results, ClaimsPrincipal user)
    {
        // prevent paging from inflating analytics
        if (query.Page > 1) return;

        if (!HasAnyFilter(query)) return;

        var signature = NormalizeQuery(query);

        var totalResults = results?.Count ?? 0;

        var ids = (results ?? Array.Empty<Comic>())
            .Select(c => c.Id)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Distinct()
            .ToList();

        var counted = ids.Take(MaxComicsToCountPerSearch).ToList();
        var truncated = ids.Count > counted.Count;

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        _db.SearchAnalyticsEvents.Add(new SearchAnalyticsEvent
        {
            OccurredUtc = DateTime.UtcNow,
            QuerySignature = signature,
            UserId = userId,
            TotalResults = totalResults,
            CountedResults = counted.Count,
            Truncated = truncated
        });

        var qs = _db.QueryStats.FirstOrDefault(x => x.Signature == signature);
        if (qs == null)
        {
            _db.QueryStats.Add(new QueryStat
            {
                Signature = signature,
                Count = 1,
                LastSeenUtc = DateTime.UtcNow
            });
        }
        else
        {
            qs.Count += 1;
            qs.LastSeenUtc = DateTime.UtcNow;
        }

        var existing = _db.ComicResultStats
            .Where(x => counted.Contains(x.ComicId))
            .ToList()
            .ToDictionary(x => x.ComicId, x => x);

        foreach (var id in counted)
        {
            if (existing.TryGetValue(id, out var stat))
            {
                stat.Count += 1;
                stat.LastSeenUtc = DateTime.UtcNow;
            }
            else
            {
                _db.ComicResultStats.Add(new ComicResultStat
                {
                    ComicId = id,
                    Count = 1,
                    LastSeenUtc = DateTime.UtcNow
                });
            }
        }

        _db.SaveChanges();
    }

    public List<SearchAnalyticsEvent> GetRecentSearches(int take = 20) =>
        _db.SearchAnalyticsEvents
            .OrderByDescending(x => x.OccurredUtc)
            .Take(take)
            .ToList();

    public List<(string Query, int Count)> GetTopQueries(int take = 10)
    {
        var rows = _db.QueryStats
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => new { x.Signature, x.Count })
            .ToList();

        return rows.Select(x => (x.Signature, x.Count)).ToList();
    }

    public List<(string ComicId, int Count)> GetTopResults(int take = 10)
    {
        var rows = _db.ComicResultStats
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => new { x.ComicId, x.Count })
            .ToList();

        return rows.Select(x => (x.ComicId, x.Count)).ToList();
    }

    public List<(string ComicId, int Count)> GetComicsOverThreshold(int threshold = 100, int take = 200)
    {
        var rows = _db.ComicResultStats
            .Where(x => x.Count > threshold)
            .OrderByDescending(x => x.Count)
            .Take(take)
            .Select(x => new { x.ComicId, x.Count })
            .ToList();

        return rows.Select(x => (x.ComicId, x.Count)).ToList();
    }

    private static bool HasAnyFilter(SearchQuery q)
    {
        static bool has(string? s) => !string.IsNullOrWhiteSpace(s);

        return has(q.TitleContains)
               || has(q.AuthorContains)
               || has(q.Genre)
               || has(q.Language)
               || has(q.Edition)
               || has(q.NameType)
               || has(q.ResourceType)
               || has(q.ContentType)
               || has(q.PhysicalDescription)
               || q.YearFrom.HasValue
               || q.YearTo.HasValue;
    }

    private static string NormalizeQuery(SearchQuery q)
    {
        static string clean(string? s) => (s ?? "").Trim().ToLowerInvariant();

        return string.Join(" | ", new[]
        {
            $"title={clean(q.TitleContains)}",
            $"author={clean(q.AuthorContains)}",
            $"genre={clean(q.Genre)}",
            $"nameType={clean(q.NameType)}",
            $"yearFrom={q.YearFrom?.ToString() ?? ""}",
            $"yearTo={q.YearTo?.ToString() ?? ""}",
            $"lang={clean(q.Language)}",
            $"edition={clean(q.Edition)}",
            $"resourceType={clean(q.ResourceType)}",
            $"contentType={clean(q.ContentType)}",
            $"phys={clean(q.PhysicalDescription)}"
        });
    }
}