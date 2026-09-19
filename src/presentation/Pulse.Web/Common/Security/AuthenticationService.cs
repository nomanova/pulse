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

    public async Task<bool> SignIn(string? username, string? password)
    {
        var request = new SignInRequest { Username = username, Password = password };
        var result = await _apiClient.Users.SignIn(request);

        if (!result.IsSuccessWithData())
        {
            return false;
        }

        var auth = result.Data!;

        await _authenticationStore.SetToken(auth.AccessToken);
        await _authenticationStore.SetUser(auth.ToUserProfile());

        _authenticationStateProvider.Notify();
        return true;
    }

    public async Task<OrganizationProfile?> GetOrganization()
    {
        return await _authenticationStore.GetOrganization();
    }

    public async Task SwitchOrganization(OrganizationProfile profile)
    {
        await _authenticationStore.SetOrganization(profile);
        _authenticationStateProvider.Notify();
    }

    public async Task<bool> SignOut()
    {
        await _authenticationStore.ClearUser();
        _authenticationStateProvider.Notify();

        return true;
    }
}