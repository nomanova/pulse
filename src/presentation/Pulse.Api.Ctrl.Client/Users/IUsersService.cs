using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.App.Dto.Users;

namespace Pulse.Api.Ctrl.Client.Users;

public interface IUsersService
{
    Task<ApiDataResult<AuthDto>> SignIn(SignInRequest request,
        CancellationToken cancellationToken = default);
}