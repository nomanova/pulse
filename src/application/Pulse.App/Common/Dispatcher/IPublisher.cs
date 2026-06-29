using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.App.Common.Dispatcher;

/// <summary>
/// Pusblishes a notification. Multiple handlers per notification are allowed.
/// </summary>
public interface IPublisher
{
    Task Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
