using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IComicVineService
{
    Task<ComicVineLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default);
}