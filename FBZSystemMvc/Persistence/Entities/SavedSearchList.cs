using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class SavedSearchList
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = default!;   // IdentityUser.Id

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public List<SavedSearchListItem> Items { get; set; } = new();
}