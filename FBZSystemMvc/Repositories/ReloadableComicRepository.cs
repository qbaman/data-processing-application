using System;
using System.Collections.Generic;
using System.Threading;
using FBZ_System.Domain;
using FBZSystemMvc.Services.DatasetUpdates;

namespace FBZ_System.Repositories;

public class ReloadableComicRepository : IComicRepository, IDatasetReloadable
{
    private readonly ReaderWriterLockSlim _lock = new();
    private IComicRepository _inner;

    public string DataFolderPath { get; }
    public DateTime LoadedUtc { get; private set; } = DateTime.UtcNow;

    public ReloadableComicRepository(string dataFolderPath)
    {
        DataFolderPath = dataFolderPath;
        _inner = new ComicRepositoryCsv(dataFolderPath);
    }

    public void Reload()
    {
        var fresh = new ComicRepositoryCsv(DataFolderPath);

        _lock.EnterWriteLock();
        try
        {
            _inner = fresh;
            LoadedUtc = DateTime.UtcNow;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    private T Read<T>(Func<IComicRepository, T> fn)
    {
        _lock.EnterReadLock();
        try { return fn(_inner); }
        finally { _lock.ExitReadLock(); }
    }

    public IReadOnlyList<Comic> GetAllComics() => Read(r => r.GetAllComics());
    public IEnumerable<string> GetAllGenres() => Read(r => r.GetAllGenres());
    public IEnumerable<string> GetAllNameTypes() => Read(r => r.GetAllNameTypes());
    public IEnumerable<string> GetAllPhysicalDescriptions() => Read(r => r.GetAllPhysicalDescriptions());
    public IEnumerable<string> GetAllResourceTypes() => Read(r => r.GetAllResourceTypes());
    public IEnumerable<string> GetAllLanguages() => Read(r => r.GetAllLanguages());
    public IEnumerable<string> GetAllEditions() => Read(r => r.GetAllEditions());
    public IEnumerable<string> GetAllTopics() => Read(r => r.GetAllTopics());
    public IEnumerable<string> GetAllContentTypes() => Read(r => r.GetAllContentTypes());
    public IReadOnlyList<Comic> GetByGenres(IEnumerable<string> genres) => Read(r => r.GetByGenres(genres));
}