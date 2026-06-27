using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Pulse.Database.Outbox.Queues;

public sealed class InMemoryQueue : IQueue, IAsyncDisposable
{
    private readonly ILogger<InMemoryQueue> _logger;

    private readonly Channel<Enqueueable> _channel;
    private readonly List<IQueueConsumer> _consumers = [];

    private readonly Lock _lock = new();
    private bool _isRunning;
    private CancellationTokenSource? _cts;
    private Task? _processingTask;

    public InMemoryQueue(ILogger<InMemoryQueue> logger)
    {
        _logger = logger;
        _channel = Channel.CreateUnbounded<Enqueueable>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public async Task Send(List<Enqueueable> items, CancellationToken cancellationToken = default)
    {
        foreach (var item in items)
        {
            await _channel.Writer.WriteAsync(item, cancellationToken);
        }

        _logger.LogInformation("Sending {BatchSize} event(s) to InMemory Queue", items.Count);
    }

    public Task StartReceiving(IQueueConsumer consumer)
    {
        lock (_lock)
        {
            if (!_consumers.Contains(consumer))
            {
                _consumers.Add(consumer);
            }

            if (_isRunning)
            {
                return Task.CompletedTask;
            }

            _cts = new CancellationTokenSource();
            _isRunning = true;
            _processingTask = Task.Run(() => ProcessQueue(_cts.Token), _cts.Token);
        }

        return Task.CompletedTask;
    }

    public Task StopReceiving(IQueueConsumer consumer)
    {
        lock (_lock)
        {
            _consumers.Remove(consumer);

            if (!_isRunning || _consumers.Count > 0)
            {
                return Task.CompletedTask;
            }

            _cts?.Cancel();
            _isRunning = false;
        }

        return Task.CompletedTask;
    }

    private async Task ProcessQueue(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var enqueueable in _channel.Reader.ReadAllAsync(cancellationToken))
            {
                IQueueConsumer[] consumersCopy;

                lock (_lock)
                {
                    consumersCopy = _consumers.ToArray();
                }

                if (consumersCopy.Length == 0)
                {
                    continue;
                }

                var dequeueable = ToDequeueable(enqueueable);

                foreach (var consumer in consumersCopy)
                {
                    try
                    {
                        // We await each consumer. If parallel processing is needed, 
                        // Task.WhenAll could be used here.
                        await consumer.OnMessageReceived(dequeueable);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while notifying consumer in InMemoryQueue");
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Critical error in InMemoryQueue background processor");
        }
    }

    private static Dequeueable? ToDequeueable(Enqueueable? enqueueable)
    {
        return enqueueable == null ? null : new Dequeueable(enqueueable.Payload);
    }

    public async ValueTask DisposeAsync()
    {
        CancellationTokenSource? cts;
        Task? processingTask;

        lock (_lock)
        {
            cts = _cts;
            processingTask = _processingTask;
            _isRunning = false;
            _cts = null;
            _processingTask = null;
        }

        if (cts != null)
        {
            await cts.CancelAsync();

            if (processingTask != null)
            {
                try
                {
                    await processingTask;
                }
                catch (OperationCanceledException)
                {
                    // NOP
                }
            }

            cts.Dispose();
        }

        _channel.Writer.TryComplete();
    }
}