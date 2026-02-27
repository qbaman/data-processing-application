using FBZSystemMvc.Persistence;
using FBZSystemMvc.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace FBZSystemMvc.Services.Persistence;

public class SavedComicsService : ISavedComicsService
{
    private readonly ApplicationDbContext _db;

    public SavedComicsService(ApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<SavedComic>> GetForUserAsync(string userId)
    {
        return await _db.SavedComics
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SavedAtUtc)
            .ToListAsync();
    }

    public async Task SaveAsync(string userId, string comicId)
    {
        var exists = await _db.SavedComics.AnyAsync(x => x.UserId == userId && x.ComicId == comicId);
        if (exists) return;

        _db.SavedComics.Add(new SavedComic { UserId = userId, ComicId = comicId, SavedAtUtc = DateTime.UtcNow });
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userId, string comicId)
    {
        var row = await _db.SavedComics.SingleOrDefaultAsync(x => x.UserId == userId && x.ComicId == comicId);
        if (row == null) return;

        _db.SavedComics.Remove(row);
        await _db.SaveChangesAsync();
    }
}