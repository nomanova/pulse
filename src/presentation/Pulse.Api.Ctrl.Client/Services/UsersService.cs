using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.App.Dto.Users;

namespace Pulse.Api.Ctrl.Client.Services;

internal sealed class UsersService : BaseService, IUsersService
{
    private const string BasePath = "/api/ctrl/v1/users";

    public UsersService(
        IEndpointProvider? endpointProvider, ITokenProvider? tokenProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, tokenProvider, httpClient)
    {
    }

    public async Task<ApiDataResult<AuthDto>> SignIn(SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/sign-in";
        return await SendForDataAsync<AuthDto>(HttpMethod.Post, url, request, cancellationToken);
    }
}