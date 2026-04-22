namespace FBZSystemMvc.Services.ExternalApis;

public class YouTubeLookupResult
{
    public List<YouTubeVideoResult> Videos { get; set; } = new();
    public bool HasData => Videos.Count > 0;
}

public class YouTubeVideoResult
{
    public string VideoId { get; set; } = "";
    public string Title { get; set; } = "";
    public string ChannelTitle { get; set; } = "";
    public string? ThumbnailUrl { get; set; }
    public string WatchUrl => $"https://www.youtube.com/watch?v={VideoId}";
}
