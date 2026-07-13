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
    private readonly IUserProvider _userProvider;
    
    public MustHaveUnchangedSecurityStampRequirementHandler(
        IUserClaimProvider userClaimProvider, 
        IUserProvider userProvider)
    {
        _userClaimProvider = userClaimProvider;
        _userProvider = userProvider;
    }
    
    public async Task<ErrorOr<Success>> Handle(MustHaveUnchangedSecurityStampRequirement request,
        CancellationToken cancellationToken)
    {
        var claimedSecurityStamp = _userClaimProvider.SecurityStamp;
        var user = await _userProvider.Get(cancellationToken);
        
        if (user.SecurityStamp.Value != claimedSecurityStamp)
        {
            return AuthorizationErrors.UserSecurityStamp;
        }

        return Result.Success;
    }
}