namespace Pulse.Application.Common.Dispatcher;

/// <summary>
/// Marker for an in-process notification. Multiple handlers per notification are allowed.
/// </summary>
public interface INotification;

public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
}

public interface IPublisher
{
    ValueTask Publish<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}