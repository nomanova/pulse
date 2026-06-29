using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Messaging.Events;

public sealed class EventArchiverBackgroundService : RecurringBackgroundService
{
    private readonly ILogger<EventArchiverBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventArchiverBackgroundService(
        ILogger<EventArchiverBackgroundService> logger,
        IOptions<MessagingOptions.EventOptions> eventOptions,
        IServiceScopeFactory serviceScopeFactory) : base(logger, eventOptions.Value.ArchiveFrequencyInSec,
        nameof(EventArchiverBackgroundService))
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteIteration(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var eventArchiver = scope.ServiceProvider.GetRequiredService<EventArchiver>();

            await eventArchiver.Execute(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Event archiver iteration failed");
        }
    }
}