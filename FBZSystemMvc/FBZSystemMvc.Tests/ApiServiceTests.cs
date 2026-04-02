using System.Net;
using System.Text;
using FBZ_System.Domain;
using FBZSystemMvc.Services.ExternalApis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FBZSystemMvc.Tests;

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/// <summary>
/// Fake HttpMessageHandler that returns a fixed status code + body.
/// Lets us test service classes without making real network calls.
/// </summary>
file sealed class FakeHttpHandler(HttpStatusCode status, string body) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(status)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}

file static class FakeHttp
{
    public static HttpClient Client(HttpStatusCode status, string body)
        => new(new FakeHttpHandler(status, body));

    public static HttpClient Client(string body)
        => Client(HttpStatusCode.OK, body);

    public static HttpClient Down()
        => Client(HttpStatusCode.ServiceUnavailable, "{}");
}

file static class FakeConfig
{
    public static IConfiguration WithKey(string section, string key, string value)
        => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { [$"{section}:{key}"] = value })
            .Build();

    public static IConfiguration Empty()
        => new ConfigurationBuilder().Build();
}

// ---------------------------------------------------------------------------
// GoogleBooksService tests
// ---------------------------------------------------------------------------

public class GoogleBooksServiceTests
{
    private static readonly Comic ComicWithIsbn = new()
    {
        Id = "1",
        MainTitle = "Watchmen",
        Isbns = ["9781779501127"],
        Authors = ["Moore, Alan"]
    };

    private static readonly Comic ComicNoIsbn = new()
    {
        Id = "2",
        MainTitle = "",
        Isbns = [],
        Authors = []
    };

    [Fact]
    public async Task LookupAsync_ValidResponse_ReturnsTitle()
    {
        const string json = """
        {
          "items": [{
            "volumeInfo": {
              "title": "Watchmen",
              "authors": ["Alan Moore"],
              "publisher": "DC Comics",
              "publishedDate": "1987",
              "pageCount": 416,
              "previewLink": "http://example.com/preview",
              "infoLink": "http://example.com/info"
            }
          }]
        }
        """;

        var svc = new GoogleBooksService(FakeHttp.Client(json), FakeConfig.Empty(), NullLogger<GoogleBooksService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.NotNull(result);
        Assert.Equal("Watchmen", result.Title);
        Assert.Contains("Alan Moore", result.Authors);
        Assert.Equal("DC Comics", result.Publisher);
        Assert.Equal(416, result.PageCount);
        Assert.True(result.HasData);
    }

    [Fact]
    public async Task LookupAsync_EmptyItemsArray_ReturnsNull()
    {
        const string json = """{ "items": [] }""";

        var svc = new GoogleBooksService(FakeHttp.Client(json), FakeConfig.Empty(), NullLogger<GoogleBooksService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAsync_ApiDown_ReturnsNullWithoutThrowing()
    {
        var svc = new GoogleBooksService(FakeHttp.Down(), FakeConfig.Empty(), NullLogger<GoogleBooksService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAsync_EmptyComic_ReturnsNullWithoutThrowing()
    {
        const string json = """{ "items": [] }""";

        var svc = new GoogleBooksService(FakeHttp.Client(json), FakeConfig.Empty(), NullLogger<GoogleBooksService>.Instance);
        var result = await svc.LookupAsync(ComicNoIsbn);

        Assert.Null(result);
    }
}

// ---------------------------------------------------------------------------
// OpenLibraryService tests
// ---------------------------------------------------------------------------

public class OpenLibraryServiceTests
{
    private static readonly Comic ComicWithIsbn = new()
    {
        Id = "1",
        MainTitle = "From Hell",
        Isbns = ["9780861661411"],
        Authors = ["Moore, Alan"]
    };

    [Fact]
    public async Task LookupAsync_ValidResponse_ReturnsTitle()
    {
        // Open Library uses snake_case keys; PropertyNameCaseInsensitive handles it for
        // single-word properties. Multi-word snake_case fields (first_publish_year) map via
        // the JsonPropertyName attributes on the model — verified here with real field names.
        const string json = """
        {
          "docs": [{
            "title": "From Hell",
            "first_publish_year": 1999,
            "author_name": ["Alan Moore"],
            "subject": ["Comics", "Horror"],
            "isbn": ["9780861661411"],
            "cover_i": 12345
          }]
        }
        """;

        var svc = new OpenLibraryService(FakeHttp.Client(json), NullLogger<OpenLibraryService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.NotNull(result);
        Assert.Equal("From Hell", result.Title);
        Assert.Equal(1999, result.FirstPublishYear);
        Assert.Contains("Alan Moore", result.AuthorNames);
        Assert.NotNull(result.CoverUrl);
        Assert.True(result.HasData);
    }

    [Fact]
    public async Task LookupAsync_EmptyDocs_ReturnsNull()
    {
        const string json = """{ "docs": [] }""";

        var svc = new OpenLibraryService(FakeHttp.Client(json), NullLogger<OpenLibraryService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAsync_ApiDown_ReturnsNullWithoutThrowing()
    {
        var svc = new OpenLibraryService(FakeHttp.Down(), NullLogger<OpenLibraryService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAsync_NoCoverI_CoverUrlIsNull()
    {
        const string json = """
        {
          "docs": [{
            "title": "From Hell",
            "first_publish_year": 1999
          }]
        }
        """;

        var svc = new OpenLibraryService(FakeHttp.Client(json), NullLogger<OpenLibraryService>.Instance);
        var result = await svc.LookupAsync(ComicWithIsbn);

        Assert.NotNull(result);
        Assert.Null(result.CoverUrl);
    }
}

// ---------------------------------------------------------------------------
// WikipediaService tests
// ---------------------------------------------------------------------------

public class WikipediaServiceTests
{
    [Fact]
    public async Task LookupAuthorAsync_ValidResponse_ReturnsBioAndImage()
    {
        const string json = """
        {
          "type": "standard",
          "title": "Alan Moore",
          "extract": "Alan Moore is an English author known primarily for his work in comic books.",
          "thumbnail": { "source": "https://upload.wikimedia.org/wikipedia/commons/thumb/alan.jpg" },
          "content_urls": {
            "desktop": { "page": "https://en.wikipedia.org/wiki/Alan_Moore" }
          }
        }
        """;

        var svc = new WikipediaService(FakeHttp.Client(json), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("Alan Moore");

        Assert.NotNull(result);
        Assert.Equal("Alan Moore", result.Title);
        Assert.Contains("comic books", result.Extract);
        Assert.NotNull(result.ThumbnailUrl);
        Assert.NotNull(result.PageUrl);
        Assert.True(result.HasData);
    }

    [Fact]
    public async Task LookupAuthorAsync_DisambiguationPage_ReturnsNull()
    {
        const string json = """
        {
          "type": "disambiguation",
          "title": "Moore",
          "extract": "Moore may refer to..."
        }
        """;

        var svc = new WikipediaService(FakeHttp.Client(json), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("Moore");

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAuthorAsync_UnknownAuthor_ReturnsNullWithoutThrowing()
    {
        var svc = new WikipediaService(FakeHttp.Client(HttpStatusCode.NotFound, "{}"), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("Zzz Nobody Known Xyz");

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAuthorAsync_ApiDown_ReturnsNullWithoutThrowing()
    {
        var svc = new WikipediaService(FakeHttp.Down(), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("Alan Moore");

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAuthorAsync_EmptyName_ReturnsNullWithoutThrowing()
    {
        var svc = new WikipediaService(FakeHttp.Down(), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("   ");

        Assert.Null(result);
    }

    [Fact]
    public async Task LookupAuthorAsync_NoThumbnail_StillReturnsBio()
    {
        const string json = """
        {
          "type": "standard",
          "title": "Alan Moore",
          "extract": "Alan Moore is an English author.",
          "content_urls": {
            "desktop": { "page": "https://en.wikipedia.org/wiki/Alan_Moore" }
          }
        }
        """;

        var svc = new WikipediaService(FakeHttp.Client(json), NullLogger<WikipediaService>.Instance);
        var result = await svc.LookupAuthorAsync("Alan Moore");

        Assert.NotNull(result);
        Assert.Null(result.ThumbnailUrl);
        Assert.True(result.HasData);
    }
}
