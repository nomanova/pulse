namespace Pulse.Application.Common.Dispatcher;

/// <summary>
/// Sends a request to its single handler through the pipeline behavior chain.
/// </summary>
public interface ISender
{
    ValueTask<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}