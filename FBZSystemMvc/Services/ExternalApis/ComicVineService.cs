using System.Net.Http;
using System.Text.Json;
using FBZ_System.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FBZSystemMvc.Services.ExternalApis;

public class ComicVineService : IComicVineService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ComicVineService> _logger;

    public ComicVineService(HttpClient httpClient, IConfiguration configuration, ILogger<ComicVineService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ComicVineLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = _configuration["ComicVine:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return null;

            var title = comic.MainTitle?.Trim();
            if (string.IsNullOrWhiteSpace(title))
                return null;

            var url =
                $"https://comicvine.gamespot.com/api/issues/" +
                $"?api_key={Uri.EscapeDataString(apiKey)}" +
                $"&format=json" +
                $"&filter=name:{Uri.EscapeDataString(title)}";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "FBZSystemMvc/1.0");

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var apiResponse = await JsonSerializer.DeserializeAsync<ComicVineApiResponse>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            var match = apiResponse?.Results?.FirstOrDefault();
            if (match is null)
                return null;

            return new ComicVineLookupResult
            {
                Name = match.Name,
                Deck = match.Deck,
                Description = match.Description,
                Publisher = match.Volume?.Name,
                ImageUrl = match.Image?.OriginalUrl,
                IssueNumber = match.IssueNumber,
                CoverDate = match.CoverDate,
                CharacterNames = match.CharacterCredits?
                    .Where(c => !string.IsNullOrWhiteSpace(c.Name))
                    .Select(c => c.Name!)
                    .Distinct()
                    .Take(10)
                    .ToList()
                    ?? new List<string>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Comic Vine lookup failed for comic '{Title}'", comic.MainTitle);
            return null;
        }
    }
}