using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.App.Common.Dispatcher;

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}