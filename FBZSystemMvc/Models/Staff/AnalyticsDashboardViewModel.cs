using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Models.Staff;

public class AnalyticsDashboardViewModel
{
    public List<SearchAnalyticsEvent> Recent { get; set; } = new();
    public List<(string QueryJson, int Count)> TopQueries { get; set; } = new();
}
