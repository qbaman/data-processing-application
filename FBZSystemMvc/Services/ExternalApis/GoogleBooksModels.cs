namespace FBZSystemMvc.Services.ExternalApis;

public class GoogleBooksLookupResult
{
    public string? Title { get; set; }
    public List<string> Authors { get; set; } = new();
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public int? PageCount { get; set; }
    public string? PreviewLink { get; set; }
    public string? InfoLink { get; set; }

    public bool HasData =>
        !string.IsNullOrWhiteSpace(Title) ||
        Authors.Count > 0 ||
        !string.IsNullOrWhiteSpace(Description) ||
        !string.IsNullOrWhiteSpace(Publisher) ||
        !string.IsNullOrWhiteSpace(PublishedDate) ||
        PageCount.HasValue ||
        !string.IsNullOrWhiteSpace(PreviewLink) ||
        !string.IsNullOrWhiteSpace(InfoLink);
}

internal class GoogleBooksApiResponse
{
    public List<GoogleBooksItem>? Items { get; set; }
}

internal class GoogleBooksItem
{
    public GoogleBooksVolumeInfo? VolumeInfo { get; set; }
}

internal class GoogleBooksVolumeInfo
{
    public string? Title { get; set; }
    public List<string>? Authors { get; set; }
    public string? Description { get; set; }
    public string? Publisher { get; set; }
    public string? PublishedDate { get; set; }
    public int? PageCount { get; set; }
    public string? PreviewLink { get; set; }
    public string? InfoLink { get; set; }
}