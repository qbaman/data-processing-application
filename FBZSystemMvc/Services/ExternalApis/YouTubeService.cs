using System.Text.Json;
using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public class YouTubeService : IYouTubeService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<YouTubeService> _logger;

    private const string BaseUrl = "https://www.googleapis.com/youtube/v3/search";

    public YouTubeService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<YouTubeService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<YouTubeLookupResult?> SearchAsync(Comic comic, CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["YouTube:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogDebug("YouTube:ApiKey not configured — skipping");
            return null;
        }

        var title = comic.MainTitle;
        if (string.IsNullOrWhiteSpace(title) || title.Length > 300)
            return null;

        var query = $"\"{title}\" comic";

        try
        {
            var url = $"{BaseUrl}?part=snippet&type=video&maxResults=5" +
                      $"&q={Uri.EscapeDataString(query)}" +
                      $"&key={apiKey}";

            using var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("YouTube API returned {Status} for '{Title}': {Body}",
                    (int)response.StatusCode, title, err);
                return null;
            }

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

            var items = doc.RootElement.GetProperty("items");
            var videos = new List<YouTubeVideoResult>();

            foreach (var item in items.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var id) ||
                    !id.TryGetProperty("videoId", out var vidId))
                    continue;

                var videoId = vidId.GetString();
                if (string.IsNullOrWhiteSpace(videoId)) continue;

                var snippet = item.GetProperty("snippet");
                var videoTitle = snippet.TryGetProperty("title", out var t) ? t.GetString() : null;
                var channel = snippet.TryGetProperty("channelTitle", out var c) ? c.GetString() : null;

                string? thumbnail = null;
                if (snippet.TryGetProperty("thumbnails", out var thumbs) &&
                    thumbs.TryGetProperty("medium", out var medium) &&
                    medium.TryGetProperty("url", out var thumbUrl))
                    thumbnail = thumbUrl.GetString();

                videos.Add(new YouTubeVideoResult
                {
                    VideoId      = videoId,
                    Title        = videoTitle ?? "(untitled)",
                    ChannelTitle = channel ?? "",
                    ThumbnailUrl = thumbnail
                });
            }

            return videos.Count > 0 ? new YouTubeLookupResult { Videos = videos } : null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "YouTube search failed for '{Title}'", title);
            return null;
        }
    }
}
