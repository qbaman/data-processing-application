using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Models.Saved;

public class SavedListViewModel
{
    public List<SavedComic> Items { get; set; } = new();
}
