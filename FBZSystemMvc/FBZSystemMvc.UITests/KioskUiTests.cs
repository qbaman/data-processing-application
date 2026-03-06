using FBZSystemMvc.UITests.Infrastructure;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace FBZSystemMvc.UITests;

[Collection("UI")]
public sealed class KioskUiTests
{
    private readonly AppServerFixture _server;
    private readonly SeleniumFixture _selenium;

    public KioskUiTests(AppServerFixture server, SeleniumFixture selenium)
    {
        _server = server;
        _selenium = selenium;
    }

    [Fact]
    public void KioskMode_Persists_After_Add_Post()
    {
        var driver = _selenium.Driver;

    driver.Manage().Cookies.DeleteAllCookies();

    driver.Navigate().GoToUrl($"{_server.BaseUrl}/Dataset?kiosk=1");

    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

    var addButton = wait.Until(d =>
    {
        var buttons = d.FindElements(By.CssSelector("form[action='/Dataset/AddToList'] button[type='submit']"));
        return buttons.FirstOrDefault(b => b.Displayed && b.Enabled);
    });

    ((IJavaScriptExecutor)driver).ExecuteScript(
        "arguments[0].scrollIntoView({block: 'center', inline: 'center'});",
        addButton
    );

    try
    {
        addButton.Click();
    }
    catch (OpenQA.Selenium.ElementClickInterceptedException)
    {
        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", addButton);
    }

    wait.Until(d => d.PageSource.Contains("Search List (", StringComparison.OrdinalIgnoreCase));

    var kioskStillOn = driver.FindElements(By.CssSelector("form[method='get'] input[name='kiosk'][value='1']")).Any();
    Assert.True(kioskStillOn, "Expected kiosk mode to still be enabled after AddToList POST.");

    Assert.Contains("kiosk=1", driver.Url, StringComparison.OrdinalIgnoreCase);
}
}