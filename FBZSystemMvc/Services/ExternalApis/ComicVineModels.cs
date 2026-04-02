using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FBZSystemMvc.Services.ExternalApis;

public class ComicVineLookupResult
{
    public bool HasData =>
        !string.IsNullOrWhiteSpace(Name) ||
        !string.IsNullOrWhiteSpace(Deck) ||
        !string.IsNullOrWhiteSpace(Description) ||
        !string.IsNullOrWhiteSpace(Publisher) ||
        !string.IsNullOrWhiteSpace(ImageUrl);

    public string? Name { get; set; }
    public string? Deck { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public string? ImageUrl { get; set; }
    public string? IssueNumber { get; set; }
    public string? CoverDate { get; set; }
    public List<string> CharacterNames { get; set; } = new();
}

public class ComicVineApiResponse
{
    [JsonPropertyName("results")]
    public List<ComicVineIssueDto>? Results { get; set; }
}

public class ComicVineIssueDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("deck")]
    public string? Deck { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("issue_number")]
    public string? IssueNumber { get; set; }

    [JsonPropertyName("cover_date")]
    public string? CoverDate { get; set; }

    [JsonPropertyName("image")]
    public ComicVineImageDto? Image { get; set; }

    [JsonPropertyName("volume")]
    public ComicVineVolumeDto? Volume { get; set; }

    [JsonPropertyName("character_credits")]
    public List<ComicVineCharacterDto>? CharacterCredits { get; set; }
}

public class ComicVineImageDto
{
    [JsonPropertyName("original_url")]
    public string? OriginalUrl { get; set; }
}

public class ComicVineVolumeDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public class ComicVineCharacterDto
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}