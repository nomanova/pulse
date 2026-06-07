using System.Linq;
using Microsoft.AspNetCore.Http;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Infra.Security.Authentication;

namespace Pulse.Infra.Security.Authorization;

public sealed class UserClaimProvider : IUserClaimProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserClaimProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string Id => GetUserClaimValue(UserClaims.Id)!;

    public string SecurityStamp => GetUserClaimValue(UserClaims.SecurityStamp)!;
    
    private string? GetUserClaimValue(string claimType, bool mustExist = true)
    {
        var value = _httpContextAccessor.HttpContext?
            .User.Claims.FirstOrDefault(claim => claim.Type == claimType)?.Value;

        if (mustExist && value is null)
        {
            throw new UnauthorizedException();
        }

        return value;
    }
}