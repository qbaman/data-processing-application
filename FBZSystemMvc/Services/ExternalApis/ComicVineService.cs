using System.Net.Http;
using System.Text.Json;
using FBZ_System.Domain;
using Microsoft.Extensions.Configuration;

namespace FBZSystemMvc.Services.ExternalApis;

public class ComicVineService : IComicVineService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ComicVineService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<ComicVineLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default)
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
}