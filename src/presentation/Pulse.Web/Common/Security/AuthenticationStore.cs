using System.Threading.Tasks;
using Pulse.App.Dto.Users;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Security;

public sealed class AuthenticationStore : IAuthenticationStore
{
    private const string TokenKey = "token";
    private const string UserKey = "user";
    private const string OrganizationKey = "organization";

    private readonly ILocalStorage _localStorage;

    public AuthenticationStore(ILocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SetToken(string token)
    {
        await _localStorage.SetItemAsStringAsync(TokenKey, token);
    }

    public async Task<string?> GetToken()
    {
        return await _localStorage.GetItemAsStringAsync(TokenKey);
    }

    public async Task SetUser(UserProfile profile)
    {
        await _localStorage.SetItemAsync(UserKey, profile);
    }

    public async Task<UserProfile?> GetUser()
    {
        return await _localStorage.GetItemAsync<UserProfile>(UserKey);
    }

    public async Task SetOrganization(OrganizationProfile profile)
    {
        await _localStorage.SetItemAsync(OrganizationKey, profile);
    }

    public async Task<OrganizationProfile?> GetOrganization()
    {
        return await _localStorage.GetItemAsync<OrganizationProfile>(OrganizationKey);
    }

    public async Task ClearUser()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
    }

    public async Task ClearAll()
    {
        await ClearUser();
        await _localStorage.RemoveItemAsync(OrganizationKey);
    }
}