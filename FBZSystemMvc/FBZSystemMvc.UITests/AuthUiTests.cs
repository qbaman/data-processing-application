using FBZSystemMvc.UITests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace FBZSystemMvc.UITests;

[Collection("UI")]
public sealed class AuthUiTests
{
    private readonly AppServerFixture _server;
    private readonly SeleniumFixture _selenium;

    public AuthUiTests(AppServerFixture server, SeleniumFixture selenium)
    {
        _server = server;
        _selenium = selenium;
    }

    [Fact]
    public void StaffAnalytics_WhenLoggedOut_RedirectsToLogin()
    {
        var driver = _selenium.Driver;

        driver.Navigate().GoToUrl($"{_server.BaseUrl}/staff/analytics");

        new WebDriverWait(driver, TimeSpan.FromSeconds(10))
            .Until(d => d.Url.Contains("Identity/Account/Login", StringComparison.OrdinalIgnoreCase)
                     || d.PageSource.Contains("Log in", StringComparison.OrdinalIgnoreCase));

        Assert.True(
            driver.Url.Contains("Identity/Account/Login", StringComparison.OrdinalIgnoreCase)
            || driver.PageSource.Contains("Log in", StringComparison.OrdinalIgnoreCase)
        );
    }
}