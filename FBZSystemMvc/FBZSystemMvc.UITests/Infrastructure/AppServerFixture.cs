using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace FBZSystemMvc.UITests.Infrastructure;

public sealed class AppServerFixture : IAsyncLifetime
{
    private Process? _process;
    public int Port { get; private set; }
    public string BaseUrl => $"http://localhost:{Port}";

    // Adjust this path if your MVC csproj lives somewhere else.
private static string WebProjectPath =>
    Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
        "../../../../FBZSystemMvc.csproj"));

    public async Task InitializeAsync()
    {
        Port = GetFreePort();

        // Build once (faster + clearer failures)
        var build = Process.Start(new ProcessStartInfo("dotnet", $"build \"{WebProjectPath}\" -c Debug")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        })!;

        var buildOut = await build.StandardOutput.ReadToEndAsync();
        var buildErr = await build.StandardError.ReadToEndAsync();

        await build.WaitForExitAsync();

        if (build.ExitCode != 0)
        {
            throw new Exception($"dotnet build failed for: {WebProjectPath}\n\nSTDOUT:\n{buildOut}\n\nSTDERR:\n{buildErr}");
        }

        // Start the web app on a known localhost URL for Selenium
        var psi = new ProcessStartInfo("dotnet",
            $"run --no-build --project \"{WebProjectPath}\" --urls \"{BaseUrl}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            Environment =
            {
                ["ASPNETCORE_ENVIRONMENT"] = "Testing"
            }
        };

        _process = Process.Start(psi);

        await WaitUntilUpAsync();
    }

    public Task DisposeAsync()
    {
        try
        {
            if (_process is { HasExited: false })
            {
                _process.Kill(entireProcessTree: true);
                _process.Dispose();
            }
        }
        catch { }

        return Task.CompletedTask;
    }

    private async Task WaitUntilUpAsync()
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };

        var deadline = DateTime.UtcNow.AddSeconds(30);
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var resp = await http.GetAsync($"{BaseUrl}/");
                if (resp.StatusCode == HttpStatusCode.OK)
                    return;
            }
            catch
            {
   
            }

            await Task.Delay(300);
        }

        var stderr = _process?.StandardError.ReadToEnd();
        throw new Exception($"App did not start within 30s. stderr:\n{stderr}");
    }

    private static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}