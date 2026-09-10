using System.Threading.Tasks;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Security;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly ICtrlApiClient _apiClient;
    private readonly UserAuthenticationStateProvider _authenticationStateProvider;
    private readonly IAuthenticationStore _authenticationStore;

    public AuthenticationService(
        ICtrlApiClient apiClient,
        UserAuthenticationStateProvider authenticationStateProvider,
        IAuthenticationStore authenticationStore)
    {
        _apiClient = apiClient;
        _authenticationStateProvider = authenticationStateProvider;
        _authenticationStore = authenticationStore;
    }

    public async Task<UserProfile?> UserProfile()
    {
        var authDto = await _authenticationStore.Get();
        if (authDto is null)
        {
            return null;
        }

        return new UserProfile
        {
            Id = authDto.User.Id,
            Username = authDto.User.Username
        };
    }

    public async Task<bool> SignIn(string? username, string? password)
    {
        var request = new SignInRequest { Username = username, Password = password };
        var result = await _apiClient.Users.SignIn(request);

        if (!result.IsSuccessWithData())
        {
            return false;
        }

        var response = result.Data!;
        await _authenticationStore.Set(response);

        _authenticationStateProvider.Notify();
        return true;
    }

    public async Task<bool> SignOut()
    {
        await _authenticationStore.Clear();
        _authenticationStateProvider.Notify();

        return true;
    }
}