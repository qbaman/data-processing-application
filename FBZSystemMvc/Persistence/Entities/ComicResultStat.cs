using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class ComicResultStat
{
    public int Id { get; set; }

    [Required]
    public string ComicId { get; set; } = default!;

    public int Count { get; set; }

    public DateTime LastSeenUtc { get; set; } = DateTime.UtcNow;
}