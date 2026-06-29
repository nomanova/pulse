using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Messaging.Events;

public class EventProcessorBackgroundService : RecurringBackgroundService
{
    private readonly ILogger<EventProcessorBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventProcessorBackgroundService(
        ILogger<EventProcessorBackgroundService> logger,
        IOptions<MessagingOptions.EventOptions> options,
        IServiceScopeFactory serviceScopeFactory) :
        base(logger, options.Value.ProcessFrequencyInSec, nameof(EventProcessorBackgroundService))
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteIteration(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var eventProcessor = scope.ServiceProvider.GetRequiredService<EventProcessor>();

            await eventProcessor.Execute(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event processor iteration failed");
        }
    }
}