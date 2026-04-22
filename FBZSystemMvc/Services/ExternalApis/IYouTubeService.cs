using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IYouTubeService
{
    Task<YouTubeLookupResult?> SearchAsync(Comic comic, CancellationToken cancellationToken = default);
}
