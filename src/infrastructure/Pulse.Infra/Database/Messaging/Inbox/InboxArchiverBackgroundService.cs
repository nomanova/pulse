using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pulse.Infra.Database.Messaging.Inbox;

public sealed class InboxArchiverBackgroundService : RecurringBackgroundService
{
    private readonly ILogger<InboxArchiverBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public InboxArchiverBackgroundService(
        ILogger<InboxArchiverBackgroundService> logger,
        IOptions<MessagingOptions.InboxOptions> options,
        IServiceScopeFactory serviceScopeFactory) : base(logger, options.Value.ArchiveFrequencyInSec,
        nameof(InboxArchiverBackgroundService))
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteIteration(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var inboxArchiver = scope.ServiceProvider.GetRequiredService<InboxArchiver>();

            await inboxArchiver.Execute(stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Inbox archiver iteration failed");
        }
    }
}