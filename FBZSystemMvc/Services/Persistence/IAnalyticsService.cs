using System.Collections.Generic;
using System.Security.Claims;
using FBZ_System.Domain;
using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Services.Persistence;

public interface IAnalyticsService
{
    void RecordSearch(SearchQuery query, IReadOnlyList<Comic> results, ClaimsPrincipal user);

    List<SearchAnalyticsEvent> GetRecentSearches(int take = 20);
    List<(string Query, int Count)> GetTopQueries(int take = 10);
    List<(string ComicId, int Count)> GetTopResults(int take = 10);
    List<(string ComicId, int Count)> GetComicsOverThreshold(int threshold = 100, int take = 200);
}