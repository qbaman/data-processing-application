using System;
using System.Linq;
using FBZ_System.Domain;
using Microsoft.EntityFrameworkCore;
using FBZSystemMvc.Data;
using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Services.Persistence;

public class SavedComicsService : ISavedComicsService
{
    private readonly ApplicationDbContext _db;

    public SavedComicsService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<SavedComic>> GetForUserAsync(string userId)
    {
        return await _db.SavedComics
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.SavedAtUtc)
            .ToListAsync();
    }

    public async Task<bool> IsSavedAsync(string userId, string comicId)
    {
        return await _db.SavedComics
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.ComicId == comicId);
    }

    public async Task SaveAsync(string userId, Comic comic)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("userId is required", nameof(userId));
        if (comic == null) throw new ArgumentNullException(nameof(comic));
        if (string.IsNullOrWhiteSpace(comic.Id)) throw new ArgumentException("Comic.Id is required", nameof(comic));

        var exists = await _db.SavedComics
            .AnyAsync(x => x.UserId == userId && x.ComicId == comic.Id);

        if (exists) return;

        var entity = new SavedComic
        {
            UserId = userId,
            ComicId = comic.Id,
            Title = comic.MainTitle ?? "",
            Authors = string.Join("; ", (comic.Authors ?? new()).Where(a => !string.IsNullOrWhiteSpace(a)).Distinct()),
            Genres = string.Join("; ", (comic.Genres ?? new()).Where(g => !string.IsNullOrWhiteSpace(g)).Distinct()),
            Years = string.Join("; ", (comic.Years ?? new()).Distinct().OrderBy(x => x)),
            Isbns = string.Join("; ", (comic.Isbns ?? new()).Where(i => !string.IsNullOrWhiteSpace(i)).Distinct()),
            SavedAtUtc = DateTime.UtcNow
        };

        _db.SavedComics.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(string userId, string comicId)
    {
        var items = await _db.SavedComics
            .Where(x => x.UserId == userId && x.ComicId == comicId)
            .ToListAsync();

        if (items.Count == 0) return;

        _db.SavedComics.RemoveRange(items);
        await _db.SaveChangesAsync();
    }

    public async Task<int> ImportFromSessionAsync(string userId, IEnumerable<Comic> comics)
    {
        var added = 0;
        foreach (var c in comics.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Id)))
        {
            var exists = await _db.SavedComics.AnyAsync(x => x.UserId == userId && x.ComicId == c.Id);
            if (exists) continue;

            await SaveAsync(userId, c);
            added++;
        }

        return added;
    }
}
