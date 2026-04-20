using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public interface IAISummaryService
{
    Task<string> GenerateSummaryAsync(
        Comic comic,
        string? externalDescription = null,
        string? wikipediaExtract = null,
        CancellationToken cancellationToken = default);
}
