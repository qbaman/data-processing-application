using FBZ_System.Domain;
using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Services.Persistence;

public interface ISavedComicsService
{
    Task<IReadOnlyList<SavedComic>> GetForUserAsync(string userId);
    Task<bool> IsSavedAsync(string userId, string comicId);
    Task SaveAsync(string userId, Comic comic);
    Task RemoveAsync(string userId, string comicId);
    Task<int> ImportFromSessionAsync(string userId, IEnumerable<Comic> comics);
}
