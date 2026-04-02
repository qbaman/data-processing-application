using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IWikipediaService
{
    Task<WikipediaLookupResult?> LookupAuthorAsync(string authorName, CancellationToken cancellationToken = default);
}
