using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pulse.Infra.Database.Messaging;

public abstract class RecurringBackgroundService : BackgroundService
{
    private readonly ILogger _logger;
    private readonly TimeSpan _frequency;
    private readonly string _serviceName;

    protected RecurringBackgroundService(
        ILogger logger,
        uint frequencyInSec,
        string serviceName)
    {
        _logger = logger;
        _frequency = TimeSpan.FromSeconds(frequencyInSec);
        _serviceName = serviceName;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {ServiceName}...", _serviceName);

        try
        {
            using var timer = new PeriodicTimer(_frequency);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExecuteIteration(stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("{ServiceName} cancelled", _serviceName);
        }
        finally
        {
            _logger.LogInformation("{ServiceName} finished", _serviceName);
        }
    }

    protected abstract Task ExecuteIteration(CancellationToken stoppingToken);
}