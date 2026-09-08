using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Pulse.App.Dto.Users;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Common.Security;

public sealed class UserAuthenticationStateProvider : AuthenticationStateProvider
{
    private const string AuthenticationScheme = "Bearer";

    private readonly ClaimsPrincipal _defaultClaimsPrincipal = new(new ClaimsIdentity());
    private readonly IAuthenticationStore _authenticationStore;

    private ClaimsPrincipal? _claimsPrincipal;

    public UserAuthenticationStateProvider(IAuthenticationStore authenticationStore)
    {
        _authenticationStore = authenticationStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_claimsPrincipal != null)
        {
            return new AuthenticationState(_claimsPrincipal);
        }

        var auth = await _authenticationStore.Get();

        _claimsPrincipal = auth == null ? _defaultClaimsPrincipal : FromAuth(auth);
        
        return new AuthenticationState(_claimsPrincipal);
    }

    public void Notify()
    {
        _claimsPrincipal = null; // Force reload
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    private static ClaimsPrincipal FromAuth(AuthDto auth)
    {
        var claims = new List<Claim>
        {
            new(type: UserClaims.UserId, value: auth.User.Id),
            new(type: UserClaims.Username, value: auth.User.Username),
        };

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        return new ClaimsPrincipal(identity);
    }
}