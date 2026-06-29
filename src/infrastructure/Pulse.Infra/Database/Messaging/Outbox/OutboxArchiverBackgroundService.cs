using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Messaging.Outbox;

public sealed class OutboxArchiverBackgroundService : RecurringBackgroundService
{
    private readonly ILogger<OutboxArchiverBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxArchiverBackgroundService(
        ILogger<OutboxArchiverBackgroundService> logger,
        IOptions<MessagingOptions.OutboxOptions> options,
        IServiceScopeFactory serviceScopeFactory) : base(logger, options.Value.ArchiveFrequencyInSec,
        nameof(OutboxArchiverBackgroundService))
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteIteration(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var outboxArchiver = scope.ServiceProvider.GetRequiredService<OutboxArchiver>();

            await outboxArchiver.Execute(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox archiver iteration failed");
        }
    }
}