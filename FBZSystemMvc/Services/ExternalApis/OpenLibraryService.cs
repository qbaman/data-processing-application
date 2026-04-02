using System.Text.Json;
using FBZ_System.Domain;
using Microsoft.Extensions.Logging;

namespace FBZSystemMvc.Services.ExternalApis;

public class OpenLibraryService : IOpenLibraryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OpenLibraryService> _logger;

    public OpenLibraryService(HttpClient httpClient, ILogger<OpenLibraryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<OpenLibraryLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default)
    {
        try
        {
            var isbn = comic.Isbns?
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x) && x.Trim().ToLower() != "missing")
                ?.Trim();

            string url;

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                url = $"https://openlibrary.org/search.json?isbn={Uri.EscapeDataString(isbn)}&limit=1";
            }
            else
            {
                var title = comic.MainTitle?.Trim() ?? "";
                var author = comic.Authors?.FirstOrDefault()?.Trim() ?? "";

                url = !string.IsNullOrWhiteSpace(author)
                    ? $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(title)}&author={Uri.EscapeDataString(author)}&limit=1"
                    : $"https://openlibrary.org/search.json?title={Uri.EscapeDataString(title)}&limit=1";
            }

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var payload = await JsonSerializer.DeserializeAsync<OpenLibrarySearchResponse>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            var doc = payload?.Docs?.FirstOrDefault();
            if (doc is null)
                return null;

            string? coverUrl = null;

            if (doc.CoverI.HasValue)
            {
                coverUrl = $"https://covers.openlibrary.org/b/id/{doc.CoverI.Value}-L.jpg?default=false";
            }

            return new OpenLibraryLookupResult
            {
                Title = doc.Title,
                FirstPublishYear = doc.FirstPublishYear,
                AuthorNames = doc.AuthorName?.Take(5).ToList() ?? new List<string>(),
                Subjects = doc.Subject?.Take(8).ToList() ?? new List<string>(),
                Isbns = doc.Isbn?.Take(5).ToList() ?? new List<string>(),
                CoverUrl = coverUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Open Library lookup failed for comic '{Title}'", comic.MainTitle);
            return null;
        }
    }
}