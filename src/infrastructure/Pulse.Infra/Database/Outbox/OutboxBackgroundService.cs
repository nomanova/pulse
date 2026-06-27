using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Outbox;

public sealed class OutboxBackgroundService : BackgroundService
{
    private readonly ILogger<OutboxBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private readonly TimeSpan _frequency;

    public OutboxBackgroundService(
        ILogger<OutboxBackgroundService> logger,
        IOptions<OutboxOptions> outboxOptions,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _frequency = TimeSpan.FromSeconds(outboxOptions.Value.FrequencyInSec);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting {ServiceName}...", nameof(OutboxBackgroundService));

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
            _logger.LogInformation("{ServiceName} cancelled", nameof(OutboxBackgroundService));
        }
        finally
        {
            _logger.LogInformation("{ServiceName} finished", nameof(OutboxBackgroundService));
        }
    }

    private async Task ExecuteIteration(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var outboxProcessor = scope.ServiceProvider.GetRequiredService<OutboxProcessor>();

            await outboxProcessor.Execute(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox processor iteration failed");
        }
    }
}