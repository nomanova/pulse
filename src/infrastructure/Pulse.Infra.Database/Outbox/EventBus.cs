using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Infra.Database.Outbox;

public interface IEventBus
{
    Task Publish(object message, CancellationToken cancellationToken = default);
}

internal sealed class EventBus : IEventBus
{
    public EventBus()
    {
    }

    public Task Publish(object message, CancellationToken cancellationToken = default)
    {
        // TODO: Implement event publishing logic

        return Task.CompletedTask;
    }
}