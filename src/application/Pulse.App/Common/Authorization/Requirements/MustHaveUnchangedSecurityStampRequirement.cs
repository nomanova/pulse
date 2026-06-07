using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Security.Interfaces;

namespace Pulse.App.Common.Authorization.Requirements;

public sealed class MustHaveUnchangedSecurityStampRequirement : IAuthorizationRequirement;

public sealed class MustHaveUnchangedSecurityStampRequirementHandler :
    IAuthorizationHandler<MustHaveUnchangedSecurityStampRequirement>
{
    private readonly IUserClaimProvider _userClaimProvider;
    private readonly ICachedUserProvider _cachedUserProvider;
    
    public MustHaveUnchangedSecurityStampRequirementHandler(
        IUserClaimProvider userClaimProvider, 
        ICachedUserProvider cachedUserProvider)
    {
        _userClaimProvider = userClaimProvider;
        _cachedUserProvider = cachedUserProvider;
    }
    
    public async Task<ErrorOr<Success>> Handle(MustHaveUnchangedSecurityStampRequirement request,
        CancellationToken cancellationToken)
    {
        var claimedSecurityStamp = _userClaimProvider.SecurityStamp;
        var user = await _cachedUserProvider.Get(cancellationToken);
        
        if (user.SecurityStamp.Value != claimedSecurityStamp)
        {
            return AuthorizationErrors.UserSecurityStamp;
        }

        return Result.Success;
    }
}