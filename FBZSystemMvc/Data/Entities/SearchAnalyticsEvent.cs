using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Data.Entities;

public class SearchAnalyticsEvent
{
    public long Id { get; set; }

    public string? UserId { get; set; }

    [Required]
    public string QueryJson { get; set; } = "{}";

    public int ResultCount { get; set; }

    public DateTime SearchedAtUtc { get; set; } = DateTime.UtcNow;
}
