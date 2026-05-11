namespace Pulse.App.Common.Dispatcher;

public interface IQuery<out TResult> : IRequest<TResult>;