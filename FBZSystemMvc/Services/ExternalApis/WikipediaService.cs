using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FBZSystemMvc.Services.ExternalApis;

public class WikipediaService : IWikipediaService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WikipediaService> _logger;

    public WikipediaService(HttpClient httpClient, ILogger<WikipediaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("FBZSystemMvc/1.0 (educational project)");
    }

    public async Task<WikipediaLookupResult?> LookupAuthorAsync(string authorName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(authorName))
            return null;

        if (authorName.Length > 300)
        {
            _logger.LogDebug("Skipping Wikipedia lookup — author name exceeds 300 characters");
            return null;
        }

        try
        {
            var url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(authorName.Trim())}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = doc.RootElement;

            // Only use articles about people, not disambiguation pages
            if (root.TryGetProperty("type", out var typeEl) && typeEl.GetString() == "disambiguation")
                return null;

            var extract = root.TryGetProperty("extract", out var e) ? e.GetString() : null;
            if (string.IsNullOrWhiteSpace(extract))
                return null;

            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;

            string? thumbnailUrl = null;
            if (root.TryGetProperty("thumbnail", out var thumb) &&
                thumb.TryGetProperty("source", out var src))
                thumbnailUrl = src.GetString();

            string? pageUrl = null;
            if (root.TryGetProperty("content_urls", out var urls) &&
                urls.TryGetProperty("desktop", out var desktop) &&
                desktop.TryGetProperty("page", out var page))
                pageUrl = page.GetString();

            return new WikipediaLookupResult
            {
                Title = title,
                Extract = extract,
                ThumbnailUrl = thumbnailUrl,
                PageUrl = pageUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Wikipedia lookup failed for author '{Author}'", authorName);
            return null;
        }
    }
}
