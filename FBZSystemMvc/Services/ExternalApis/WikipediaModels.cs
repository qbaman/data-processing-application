namespace FBZSystemMvc.Services.ExternalApis;

public class WikipediaLookupResult
{
    public string? Title { get; set; }
    public string? Extract { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? PageUrl { get; set; }
    public bool HasData => !string.IsNullOrWhiteSpace(Extract);
}
