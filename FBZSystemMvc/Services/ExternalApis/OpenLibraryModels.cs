namespace FBZSystemMvc.Services.ExternalApis;

public class OpenLibraryLookupResult
{
    public string? Title { get; set; }
    public int? FirstPublishYear { get; set; }
    public List<string> AuthorNames { get; set; } = new();
    public List<string> Subjects { get; set; } = new();
    public List<string> Isbns { get; set; } = new();
    public string? CoverUrl { get; set; }

    public bool HasData =>
        !string.IsNullOrWhiteSpace(Title) ||
        FirstPublishYear.HasValue ||
        AuthorNames.Count > 0 ||
        Subjects.Count > 0 ||
        Isbns.Count > 0 ||
        !string.IsNullOrWhiteSpace(CoverUrl);
}

internal class OpenLibrarySearchResponse
{
    public List<OpenLibraryDoc>? Docs { get; set; }
}

internal class OpenLibraryDoc
{
    public string? Title { get; set; }
    public int? FirstPublishYear { get; set; }
    public List<string>? AuthorName { get; set; }
    public List<string>? Subject { get; set; }
    public List<string>? Isbn { get; set; }
    public int? CoverI { get; set; }
}