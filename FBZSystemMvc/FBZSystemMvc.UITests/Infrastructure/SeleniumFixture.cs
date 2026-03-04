using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace FBZSystemMvc.UITests.Infrastructure;

public sealed class SeleniumFixture : IDisposable
{
    public IWebDriver Driver { get; }

    public SeleniumFixture()
    {
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-gpu");

        Driver = new ChromeDriver(options);
    }

    public void Dispose()
    {
        try { Driver.Quit(); } catch { }
        Driver.Dispose();
    }
}