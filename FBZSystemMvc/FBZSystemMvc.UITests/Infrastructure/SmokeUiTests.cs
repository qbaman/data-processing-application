using FBZSystemMvc.UITests.Infrastructure;
using OpenQA.Selenium;
using Xunit;

namespace FBZSystemMvc.UITests;

[Collection("UI")]
public sealed class SmokeUiTests
{
    private readonly AppServerFixture _server;
    private readonly SeleniumFixture _selenium;

    public SmokeUiTests(AppServerFixture server, SeleniumFixture selenium)
    {
        _server = server;
        _selenium = selenium;
    }

    [Fact]
    public void HomePage_Loads()
    {
        _selenium.Driver.Navigate().GoToUrl($"{_server.BaseUrl}/");
        Assert.Contains("FBZ", _selenium.Driver.Title, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void DatasetPage_Loads()
    {
        _selenium.Driver.Navigate().GoToUrl($"{_server.BaseUrl}/Dataset");
        var h1 = _selenium.Driver.FindElement(By.TagName("h1")).Text;
        Assert.Contains("Dataset", h1, StringComparison.OrdinalIgnoreCase);
    }
}