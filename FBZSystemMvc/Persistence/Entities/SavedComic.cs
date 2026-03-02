using System.ComponentModel.DataAnnotations;
using FBZSystemMvc.Persistence;

namespace FBZSystemMvc.Persistence.Entities;

public class SavedComic
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    // The BL/CSV "Id" you already use in Domain.Comic
    [Required]
    public string ComicId { get; set; } = string.Empty;

    public DateTime SavedAtUtc { get; set; } = DateTime.UtcNow;
}