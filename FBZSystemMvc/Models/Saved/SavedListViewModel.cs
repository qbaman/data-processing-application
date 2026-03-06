using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Models.Saved;


public class SavedListViewModel
{
    public List<SavedComic> Items { get; set; } = new();

    public IReadOnlyList<SavedComic> SavedComics
    {
        get => Items;
        init => Items = value?.ToList() ?? new List<SavedComic>();
    }
}