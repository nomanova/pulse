using System.Threading.Tasks;
using Pulse.App.Dto.Users;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Security;

public sealed class AuthenticationStore : IAuthenticationStore
{
    private const string AuthKey = "auth";

    private readonly ILocalStorage _localStorage;

    public AuthenticationStore(ILocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task Set(AuthDto auth)
    {
        await _localStorage.SetItemAsync(AuthKey, auth);
    }

    public async Task<AuthDto?> Get()
    {
        return await _localStorage.GetItemAsync<AuthDto>(AuthKey);
    }

    public async Task Clear()
    {
        await _localStorage.RemoveItemAsync(AuthKey);
    }
}