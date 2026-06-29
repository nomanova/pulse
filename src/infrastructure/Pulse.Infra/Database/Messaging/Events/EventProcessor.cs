using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Models.Events;
using Pulse.Domain.Common.Services;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Events;

/**
 * The implementation provides at-least-once processing with a single active claim per event.
 * To make event handling effectively exactly once, each handler should be idempotent.
 */
public sealed class EventProcessor
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly DatabaseContext _context;
    private readonly DatabaseOptions _databaseOptions;
    private readonly ILogger<EventProcessor> _logger;
    private readonly IPublisher _publisher;

    private readonly uint _batchSize;
    private readonly TimeSpan _claimTimeout;

    public EventProcessor(
        IDateTimeProvider dateTimeProvider,
        DatabaseContext context,
        IOptions<DatabaseOptions> databaseOptions,
        IOptions<MessagingOptions.EventOptions> eventOptions,
        ILogger<EventProcessor> logger,
        IPublisher publisher)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
        _databaseOptions = databaseOptions.Value;
        _logger = logger;
        _publisher = publisher;

        _batchSize = eventOptions.Value.ProcessBatchSize;
        _claimTimeout = TimeSpan.FromMinutes(eventOptions.Value.ClaimTimeoutInMin);
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var processingId = IdentityProvider.New();
        var now = _dateTimeProvider.UtcNow;
        var claimExpiredBefore = now.Subtract(_claimTimeout);

        var claimedEvents = await ClaimEvents(
            processingId,
            now,
            claimExpiredBefore,
            cancellationToken);

        if (claimedEvents.Count == 0)
        {
            return;
        }

        foreach (var @event in claimedEvents)
        {
            try
            {
                await PublishEvent(@event, cancellationToken);

                @event.ProcessedOn = _dateTimeProvider.UtcNow;
                @event.ProcessingId = null;
                @event.ProcessingOn = null;
                @event.Error = null;

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Processed event {EventId} of type {EventType}",
                    @event.Id,
                    @event.Type);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                @event.ProcessingId = null;
                @event.ProcessingOn = null;
                @event.Error = ex.ToString();

                try
                {
                    await _context.SaveChangesAsync(CancellationToken.None);
                }
                catch (Exception saveException)
                {
                    _logger.LogError(
                        saveException,
                        "Failed to release claimed event {EventId} after handler failure",
                        @event.Id);
                }

                _logger.LogError(
                    ex,
                    "Failed to process event {EventId} of type {EventType}",
                    @event.Id,
                    @event.Type);
            }
        }
    }

    private async Task<List<Event>> ClaimEvents(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        return _databaseOptions.Provider switch
        {
            DatabaseProvider.Postgres => await ClaimPostgresEvents(
                processingId,
                processingOn,
                claimExpiredBefore,
                cancellationToken),

            DatabaseProvider.Sqlite => await ClaimSqliteEvents(
                processingId,
                processingOn,
                claimExpiredBefore,
                cancellationToken),

            _ => throw new NotSupportedException(
                $"Database provider '{_databaseOptions.Provider}' is not supported by the event processor")
        };
    }

    private async Task<List<Event>> ClaimPostgresEvents(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var events = await _context.Events
            .FromSqlInterpolated($"""
                                  WITH claimed AS (
                                      SELECT id
                                      FROM events
                                      WHERE processed_on IS NULL
                                        AND (
                                            processing_on IS NULL
                                            OR processing_on < {claimExpiredBefore}
                                        )
                                      ORDER BY occurred_on
                                      LIMIT {_batchSize}
                                      FOR UPDATE SKIP LOCKED
                                  )
                                  UPDATE events event
                                  SET processing_id = {processingId},
                                      processing_on = {processingOn}
                                  FROM claimed
                                  WHERE event.id = claimed.id
                                  RETURNING event.*
                                  """)
            .ToListAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return events;
    }

    private async Task<List<Event>> ClaimSqliteEvents(
        string processingId,
        DateTime processingOn,
        DateTime claimExpiredBefore,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var events = await _context.Events
            .FromSqlInterpolated($"""
                                  UPDATE events
                                  SET processing_id = {processingId},
                                      processing_on = {processingOn}
                                  WHERE id IN (
                                      SELECT id
                                      FROM events
                                      WHERE processed_on IS NULL
                                        AND (
                                            processing_on IS NULL
                                            OR processing_on < {claimExpiredBefore}
                                        )
                                      ORDER BY occurred_on
                                      LIMIT {_batchSize}
                                  )
                                  RETURNING *
                                  """)
            .ToListAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return events;
    }

    private async Task PublishEvent(
        Event @event,
        CancellationToken cancellationToken)
    {
        var eventType = ResolveEventType(@event.Type);

        if (eventType is null)
        {
            throw new InvalidOperationException(
                $"Event type '{@event.Type}' could not be resolved.");
        }

        if (!typeof(INotification).IsAssignableFrom(eventType))
        {
            throw new InvalidOperationException(
                $"Event type '{@event.Type}' does not implement {nameof(INotification)}.");
        }

        var notification = JsonSerializer.Deserialize(@event.Content, eventType);

        if (notification is null)
        {
            throw new InvalidOperationException(
                $"Event '{@event.Id}' of type '{@event.Type}' could not be deserialized.");
        }

        await PublishDynamic((dynamic)notification, cancellationToken);
    }

    private static Type? ResolveEventType(string eventType)
    {
        return Type.GetType(eventType, throwOnError: false)
               ?? AppDomain.CurrentDomain
                   .GetAssemblies()
                   .Select(assembly => assembly.GetType(eventType, throwOnError: false))
                   .FirstOrDefault(type => type is not null);
    }

    private Task PublishDynamic<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken)
        where TNotification : INotification
    {
        return _publisher.Publish(notification, cancellationToken);
    }
}