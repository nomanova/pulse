namespace Pulse.App.Common.Dispatcher;

public interface ICommand : IRequest;

public interface ICommand<out TResult> : IRequest<TResult>;