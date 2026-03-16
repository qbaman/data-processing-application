using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IGoogleBooksService
{
    Task<GoogleBooksLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default);
}