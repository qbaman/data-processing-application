using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class FlaggedComic
{
    public int Id { get; set; }

    [Required]
    public string ComicId { get; set; } = default!;

    [Required]
    public string StaffUserId { get; set; } = default!;

    public string? Reason { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}