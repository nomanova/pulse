using ErrorOr;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App.Common.Authorization;

public interface IAuthorizationHandler<in TRequest> : IRequestHandler<TRequest, ErrorOr<Success>>
    where TRequest : IRequest<ErrorOr<Success>>;