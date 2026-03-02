using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Models.Saved;

/// <summary>
/// Saved list view model.
/// Controller expects an assignable Items property.
/// </summary>
public class SavedListViewModel
{
    public List<SavedComic> Items { get; set; } = new();

    // Optional alias for other code paths
    public IReadOnlyList<SavedComic> SavedComics
    {
        get => Items;
        init => Items = value?.ToList() ?? new List<SavedComic>();
    }
}