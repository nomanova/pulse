namespace Pulse.Application.Common.Dispatcher;

public interface IQuery<out TResult> : IRequest<TResult>;