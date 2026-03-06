using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class SearchAnalyticsEvent
{
    public int Id { get; set; }

    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public string QuerySignature { get; set; } = default!;

    public string? UserId { get; set; } 

    public int TotalResults { get; set; }

    public int CountedResults { get; set; }

    public bool Truncated { get; set; }
}