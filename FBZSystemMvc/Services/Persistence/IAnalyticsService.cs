namespace FBZSystemMvc.Services.Persistence;

public interface IAnalyticsService
{
    Task RecordSearchAsync(string queryText, int resultCount, string? userId, IEnumerable<string>? resultComicIds = null);
}