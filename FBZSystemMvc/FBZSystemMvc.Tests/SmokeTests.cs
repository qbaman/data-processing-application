using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FBZSystemMvc.Tests;

public class SmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public SmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });
    }

    [Fact]
    public async Task Home_Page_Loads()
    {
        var res = await _client.GetAsync("/");
        res.EnsureSuccessStatusCode();

        var html = await res.Content.ReadAsStringAsync();
        Assert.Contains("Fantasy", html);
    }

    [Fact]
    public async Task Dataset_Page_Loads()
    {
        var res = await _client.GetAsync("/Dataset?Page=1&PageSize=25");
        res.EnsureSuccessStatusCode();

        var html = await res.Content.ReadAsStringAsync();
        Assert.Contains("Dataset Search", html);
    }

    [Fact]
    public async Task Kiosk_Mode_Renders()
    {
        var res = await _client.GetAsync("/Dataset?kiosk=1&Page=1&PageSize=25");
        res.EnsureSuccessStatusCode();

        var html = await res.Content.ReadAsStringAsync();
        Assert.Contains("Kiosk mode", html);
    }
}