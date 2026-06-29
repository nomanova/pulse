using System;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.App.Common.Dispatcher;

public abstract class IdempotentNotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    private readonly IInbox _inbox;
    private readonly INotificationContext _notificationContext;

    protected IdempotentNotificationHandler(
        IInbox inbox,
        INotificationContext notificationContext)
    {
        _inbox = inbox;
        _notificationContext = notificationContext;
    }

    public Task Handle(
        TNotification notification,
        CancellationToken cancellationToken)
    {
        var notificationId = _notificationContext.NotificationId;

        if (string.IsNullOrWhiteSpace(notificationId))
        {
            throw new InvalidOperationException(
                "Idempotent notification handlers require a notification id.");
        }

        return _inbox.ExecuteIdempotently(
            notificationId,
            GetType().FullName!,
            token => HandleIdempotently(notification, token),
            cancellationToken);
    }

    protected abstract Task HandleIdempotently(
        TNotification notification,
        CancellationToken cancellationToken);
}