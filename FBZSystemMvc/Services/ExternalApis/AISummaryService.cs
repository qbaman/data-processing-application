using System.Text;
using System.Text.Json;
using FBZ_System.Domain;

namespace FBZSystemMvc.Services.ExternalApis;

public class AISummaryService : IAISummaryService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AISummaryService> _logger;

    private const string Endpoint = "https://api.openai.com/v1/chat/completions";
    private const string Model    = "gpt-4o-mini";

    private const string Unavailable =
        "AI summary is temporarily unavailable.";
    private const string Insufficient =
        "There is not enough information available to generate a meaningful summary for this comic.";

    public AISummaryService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<AISummaryService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _httpClient.Timeout = TimeSpan.FromSeconds(20);
    }

    public async Task<string> GenerateSummaryAsync(
        Comic comic,
        string? externalDescription = null,
        string? wikipediaExtract = null,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["OpenAI:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogWarning("OpenAI:ApiKey is not configured — AI summary skipped");
            return Unavailable;
        }

        var prompt = BuildPrompt(comic, externalDescription, wikipediaExtract);
        if (prompt is null)
            return Insufficient;

        try
        {
            var body = JsonSerializer.Serialize(new
            {
                model = Model,
                messages = new[]
                {
                    new
                    {
                        role    = "system",
                        content = "You are a helpful assistant for a public comic book library. " +
                                  "Write concise, engaging catalogue summaries suitable for general audiences."
                    },
                    new { role = "user", content = prompt }
                },
                max_tokens  = 160,
                temperature = 0.7
            });

            using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogWarning("OpenAI returned {Status} for '{Title}': {Body}",
                    (int)response.StatusCode, comic.MainTitle, err);
                return Unavailable;
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return string.IsNullOrWhiteSpace(content) ? Unavailable : content.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "OpenAI call failed for comic '{Title}'", comic.MainTitle);
            return Unavailable;
        }
    }

    private static string? BuildPrompt(Comic comic, string? externalDescription, string? wikipediaExtract)
    {
        var authors = (comic.Authors ?? Enumerable.Empty<string>())
            .Where(a => !string.IsNullOrWhiteSpace(a)).ToList();
        var years   = (comic.Years   ?? Enumerable.Empty<int>()).OrderBy(y => y).ToList();
        var genres  = (comic.Genres  ?? Enumerable.Empty<string>())
            .Where(g => !string.IsNullOrWhiteSpace(g)).ToList();

        var topics = new List<string>();
        if (comic.ExtraAttributes?.TryGetValue("Topics", out var topicList) == true && topicList is not null)
            topics = topicList.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

        // Require at least one meaningful data point beyond the title
        bool hasData = authors.Count > 0
            || genres.Count > 0
            || topics.Count > 0
            || !string.IsNullOrWhiteSpace(externalDescription)
            || !string.IsNullOrWhiteSpace(wikipediaExtract);

        if (!hasData) return null;

        var sb = new StringBuilder();
        sb.AppendLine(
            "Write a concise 2–3 sentence summary for the comic below, " +
            "suitable for a library catalogue. Use only the information provided; " +
            "do not invent details.");
        sb.AppendLine();
        sb.AppendLine($"Title: {comic.MainTitle}");

        if (authors.Count > 0)
            sb.AppendLine($"Author(s): {string.Join(", ", authors)}");
        if (years.Count > 0)
            sb.AppendLine($"Year(s): {string.Join(", ", years)}");
        if (genres.Count > 0)
            sb.AppendLine($"Genre(s): {string.Join(", ", genres)}");
        if (topics.Count > 0)
            sb.AppendLine($"Topics: {string.Join(", ", topics)}");
        if (!string.IsNullOrWhiteSpace(externalDescription))
            sb.AppendLine($"Description: {Truncate(externalDescription, 500)}");
        if (!string.IsNullOrWhiteSpace(wikipediaExtract))
            sb.AppendLine($"Author background: {Truncate(wikipediaExtract, 300)}");

        return sb.ToString();
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";
}
