using System.ComponentModel.DataAnnotations;
using FBZSystemMvc.Persistence;
using Microsoft.EntityFrameworkCore;


namespace FBZSystemMvc.Persistence.Entities;

public class SearchAnalyticsEvent
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    public string QueryText { get; set; } = string.Empty;

    public int ResultCount { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<SearchResultHit> ResultHits { get; set; } = new();
}