using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Messaging.Outbox;

public sealed class OutboxProcessorBackgroundService : RecurringBackgroundService
{
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxProcessorBackgroundService(
        ILogger<OutboxProcessorBackgroundService> logger,
        IOptions<MessagingOptions.OutboxOptions> outboxOptions,
        IServiceScopeFactory serviceScopeFactory) :
        base(logger, outboxOptions.Value.ProcessFrequencyInSec, nameof(OutboxProcessorBackgroundService))
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteIteration(CancellationToken stoppingToken)
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