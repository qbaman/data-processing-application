using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class QueryStat
{
    public int Id { get; set; }

    [Required]
    public string Signature { get; set; } = default!;

    public int Count { get; set; }

    public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
}