using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Pulse.Infra.Database.Outbox.Queues;
using Xunit;

namespace Pulse.Infra.Tests.Database.Outbox.Queues;

public class InMemoryQueueTests
{
    [Fact]
    public async Task Send_ShouldDeliverMessageToConsumer()
    {
        // Arrange
        var queue = new InMemoryQueue(NullLogger<InMemoryQueue>.Instance);
        var consumer = new TestConsumer();
        await queue.StartReceiving(consumer);

        var item = new Enqueueable("payload", "subject", "id", "pk", "sid");

        // Act
        await queue.Send([item]);

        // Assert
        // Give it a bit of time to process
        for (var i = 0; i < 10; i++)
        {
            if (consumer.ReceivedMessages.Count > 0) break;
            await Task.Delay(50);
        }
        
        Assert.Single(consumer.ReceivedMessages);
        Assert.Equal("payload", consumer.ReceivedMessages[0].Payload);
        
        await queue.DisposeAsync();
    }

    [Fact]
    public async Task StartStopReceiving_ShouldBeThreadSafe()
    {
        // Arrange
        var queue = new InMemoryQueue(NullLogger<InMemoryQueue>.Instance);
        var consumers = new List<TestConsumer>();
        for (int i = 0; i < 10; i++) consumers.Add(new TestConsumer());

        // Act
        var tasks = new List<Task>();
        for (var i = 0; i < 100; i++)
        {
            var c = consumers[i % 10];
            tasks.Add(Task.Run(async () =>
            {
                await queue.StartReceiving(c);
                await queue.StopReceiving(c);
            }));
        }

        // Assert
        var exception = await Record.ExceptionAsync(Act);
        Assert.Null(exception);
        
        await queue.DisposeAsync();
        return;

        async Task Act() => await Task.WhenAll(tasks);
    }

    [Fact]
    public async Task Send_WhenMultipleConsumers_ShouldDeliverToAll()
    {
        // Arrange
        var queue = new InMemoryQueue(NullLogger<InMemoryQueue>.Instance);
        var consumer1 = new TestConsumer();
        var consumer2 = new TestConsumer();
        await queue.StartReceiving(consumer1);
        await queue.StartReceiving(consumer2);

        var item = new Enqueueable("payload", "subject", "id", "pk", "sid");

        // Act
        await queue.Send([item]);

        // Assert
        for (int i = 0; i < 10; i++)
        {
            if (consumer1.ReceivedMessages.Count > 0 && consumer2.ReceivedMessages.Count > 0) break;
            await Task.Delay(50);
        }

        Assert.Single(consumer1.ReceivedMessages);
        Assert.Single(consumer2.ReceivedMessages);
        
        await queue.DisposeAsync();
    }

    private class TestConsumer : IQueueConsumer
    {
        private readonly List<Dequeueable> _messages = new();
        private readonly Lock _lock = new();

        public List<Dequeueable> ReceivedMessages 
        { 
            get 
            {
                lock (_lock) return [.._messages];
            }
        }

        public Task OnMessageReceived(Dequeueable? item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    _messages.Add(item);
                }
            }
            return Task.CompletedTask;
        }
    }
}
