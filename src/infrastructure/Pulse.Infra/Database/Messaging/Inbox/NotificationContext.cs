using Pulse.App.Common.Dispatcher;

namespace Pulse.Infra.Database.Messaging.Inbox;

public sealed class NotificationContext : INotificationContext
{
    public string? NotificationId { get; private set; }

    public void SetNotificationId(string notificationId)
    {
        NotificationId = notificationId;
    }

    public void Clear()
    {
        NotificationId = null;
    }
}