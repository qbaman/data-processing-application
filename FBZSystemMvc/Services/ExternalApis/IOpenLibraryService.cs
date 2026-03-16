using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IOpenLibraryService
{
    Task<OpenLibraryLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default);
}