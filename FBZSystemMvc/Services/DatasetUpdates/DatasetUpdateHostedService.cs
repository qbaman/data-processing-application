using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FBZSystemMvc.Services.DatasetUpdates;

public class DatasetUpdateHostedService : BackgroundService
{
    private readonly IDatasetUpdateService _updates;
    private readonly DatasetUpdateOptions _opts;
    private readonly ILogger<DatasetUpdateHostedService> _log;

    public DatasetUpdateHostedService(
        IDatasetUpdateService updates,
        IOptions<DatasetUpdateOptions> opts,
        ILogger<DatasetUpdateHostedService> log)
    {
        _updates = updates;
        _opts = opts.Value;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _updates.CheckAndUpdateAsync(force: false, stoppingToken);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Background dataset check failed");
            }

            await Task.Delay(TimeSpan.FromHours(_opts.CheckEveryHours), stoppingToken);
        }
    }
}