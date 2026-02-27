using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Models.Staff;

/// <summary>
/// View model for the Staff analytics dashboard.
/// Keeps property names compatible with controller/view code.
/// </summary>
public class AnalyticsDashboardViewModel
{
    public List<SearchAnalyticsEvent> RecentSearches { get; set; } = new();
    public List<(string Query, int Count)> TopQueries { get; set; } = new();
    public List<(string ComicId, int Count)> TopResults { get; set; } = new();
    public List<(string ComicId, int Count)> ComicsOver100 { get; set; } = new();

    // Backwards-compatible alias (in case any older view still uses vm.Recent)
    public List<SearchAnalyticsEvent> Recent
    {
        get => RecentSearches;
        set => RecentSearches = value;
    }
}