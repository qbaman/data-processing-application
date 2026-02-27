using System.ComponentModel.DataAnnotations;
using FBZSystemMvc.Persistence;

namespace FBZSystemMvc.Persistence.Entities;

public class SearchResultHit
{
    public int Id { get; set; }

    public int SearchAnalyticsEventId { get; set; }
    public SearchAnalyticsEvent? SearchAnalyticsEvent { get; set; }

    [Required]
    public string ComicId { get; set; } = string.Empty;
}