using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Services.Persistence;

public interface ISavedComicsService
{
    Task<IReadOnlyList<SavedComic>> GetForUserAsync(string userId);
    Task SaveAsync(string userId, string comicId);
    Task RemoveAsync(string userId, string comicId);
}