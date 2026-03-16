using System.Text.Json;
using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public class GoogleBooksService : IGoogleBooksService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public GoogleBooksService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<GoogleBooksLookupResult?> LookupAsync(Comic comic, CancellationToken cancellationToken = default)
    {
        try
        {
            var apiKey = _configuration["GoogleBooks:ApiKey"];

            var isbn = comic.Isbns?
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x) && x.Trim().ToLower() != "missing")
                ?.Trim();

            string q;

            if (!string.IsNullOrWhiteSpace(isbn))
            {
                q = $"isbn:{isbn}";
            }
            else
            {
                var title = comic.MainTitle?.Trim() ?? "";
                var author = comic.Authors?.FirstOrDefault()?.Trim() ?? "";

                q = !string.IsNullOrWhiteSpace(author)
                    ? $"intitle:{title} inauthor:{author}"
                    : $"intitle:{title}";
            }

            var url = $"https://www.googleapis.com/books/v1/volumes?q={Uri.EscapeDataString(q)}&maxResults=1";

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                url += $"&key={Uri.EscapeDataString(apiKey)}";
            }

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return null;

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var payload = await JsonSerializer.DeserializeAsync<GoogleBooksApiResponse>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken);

            var volume = payload?.Items?.FirstOrDefault()?.VolumeInfo;
            if (volume is null)
                return null;

            return new GoogleBooksLookupResult
            {
                Title = volume.Title,
                Authors = volume.Authors ?? new List<string>(),
                Description = volume.Description,
                Publisher = volume.Publisher,
                PublishedDate = volume.PublishedDate,
                PageCount = volume.PageCount,
                PreviewLink = volume.PreviewLink,
                InfoLink = volume.InfoLink
            };
        }
        catch
        {
            return null;
        }
    }
}