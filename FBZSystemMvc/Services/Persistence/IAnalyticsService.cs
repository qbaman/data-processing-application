using FBZ_System.Domain;
using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Services.Persistence;

public interface IAnalyticsService
{
    Task RecordSearchAsync(SearchQuery query, int resultCount, string? userId);
    Task<IReadOnlyList<SearchAnalyticsEvent>> GetRecentAsync(int take = 250);
    Task<IReadOnlyList<(string QueryJson, int Count)>> GetTopQueriesAsync(int take = 25);
}
