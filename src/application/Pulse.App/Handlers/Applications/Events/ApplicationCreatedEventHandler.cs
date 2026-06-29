using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Dispatcher;
using Pulse.Domain.Aggregates.Applications.Events;

namespace Pulse.App.Handlers.Applications.Events;

public class ApplicationCreatedEventHandler : IdempotentNotificationHandler<ApplicationCreatedEvent>
{
    public ApplicationCreatedEventHandler(
        IInbox inbox, INotificationContext notificationContext) : base(inbox, notificationContext)
    {
    }

    protected override async Task HandleIdempotently(ApplicationCreatedEvent notification, CancellationToken cancellationToken)
    {
        // Handler logic here.
        await Task.CompletedTask;
    }
}