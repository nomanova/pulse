namespace Pulse.Application.Common.Dispatcher;

/// <summary>
/// Marker for a request that returns a response.
/// </summary>
public interface IRequest<out TResponse>;

/// <summary>
/// Marker for a request that returns no payload. Returns <see cref="Unit"/>.
/// </summary>
public interface IRequest : IRequest<Unit>;

/// <summary>
/// Void return type for commands that have no response payload.
/// </summary>
public readonly struct Unit
{
    public static readonly Unit Value = default;
}