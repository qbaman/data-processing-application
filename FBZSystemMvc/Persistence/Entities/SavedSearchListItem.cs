using System.ComponentModel.DataAnnotations;

namespace FBZSystemMvc.Persistence.Entities;

public class SavedSearchListItem
{
    public int Id { get; set; }

    public int SavedSearchListId { get; set; }
    public SavedSearchList SavedSearchList { get; set; } = default!;

    [Required]
    public string ComicId { get; set; } = default!;
}