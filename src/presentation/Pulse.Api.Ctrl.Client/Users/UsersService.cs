using Pulse.Api.Client;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Services;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.App.Dto.Users;

namespace Pulse.Api.Ctrl.Client.Users;

internal sealed class UsersService : BaseService, IUsersService
{
    private const string BasePath = "/api/ctrl/v1/users";

    public UsersService(IApiEndpointProvider? endpointProvider, ApiHttpClient? httpClient)
        : base(endpointProvider, httpClient, BasePath)
    {
    }

    public async Task<ApiDataResult<AuthDto>> SignIn(SignInRequest request,
        CancellationToken cancellationToken = default)
    {
        const string url = $"{BasePath}/sign-in";
        return await SendForDataAsync<AuthDto>(HttpMethod.Post, url, request, cancellationToken);
    }
}