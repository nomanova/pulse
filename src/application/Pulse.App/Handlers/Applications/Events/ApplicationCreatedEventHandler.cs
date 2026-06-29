using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Dispatcher;
using Pulse.Domain.Aggregates.Applications.Events;

namespace Pulse.App.Handlers.Applications.Events;

public class ApplicationCreatedEventHandler : INotificationHandler<ApplicationCreatedEvent>
{
    public async Task Handle(ApplicationCreatedEvent notification, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
    }
}