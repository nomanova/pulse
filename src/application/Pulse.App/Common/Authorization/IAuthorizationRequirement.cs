using ErrorOr;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App.Common.Authorization;

public interface IAuthorizationRequirement : IRequest<ErrorOr<Success>>;