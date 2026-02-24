using System;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Data.Entities;

public class SavedComic
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// The in-memory dataset identifier (BL record ID in your CSV-backed model).
    /// </summary>
    [Required]
    [MaxLength(128)]
    public string ComicId { get; set; } = string.Empty;

    [Required]
    [MaxLength(512)]
    public string Title { get; set; } = string.Empty;

    // Stored as display-friendly strings to keep persistence simple.
    public string Authors { get; set; } = string.Empty;
    public string Genres { get; set; } = string.Empty;
    public string Years { get; set; } = string.Empty;
    public string Isbns { get; set; } = string.Empty;

    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}
