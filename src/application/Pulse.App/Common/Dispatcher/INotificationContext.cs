namespace Pulse.App.Common.Dispatcher;

public interface INotificationContext
{
    string? NotificationId { get; }

    void SetNotificationId(string notificationId);

    void Clear();
}