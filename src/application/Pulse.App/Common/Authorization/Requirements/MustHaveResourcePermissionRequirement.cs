using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Context;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Memberships.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;

namespace Pulse.App.Common.Authorization.Requirements;

public sealed class MustHaveResourcePermissionRequirement : IAuthorizationRequirement;

public sealed class MustHaveResourcePermissionRequirementHandler :
    IAuthorizationHandler<MustHaveResourcePermissionRequirement>
{
    private readonly IUserProvider _userProvider;
    private readonly IContextProvider _contextProvider;
    private readonly IMembershipRepository _membershipRepository;

    public MustHaveResourcePermissionRequirementHandler(
        IUserProvider userProvider,
        IContextProvider contextProvider,
        IMembershipRepository membershipRepository)
    {
        _userProvider = userProvider;
        _contextProvider = contextProvider;
        _membershipRepository = membershipRepository;
    }

    public async Task<ErrorOr<Success>> Handle(MustHaveResourcePermissionRequirement request,
        CancellationToken cancellationToken)
    {
        var user = await _userProvider.Get(cancellationToken);

        var specification = new UserMembershipsSpecification(user.Id);
        var memberships = await _membershipRepository.Search(specification, cancellationToken);

        if (memberships.IsSrvOwner())
        {
            return Result.Success;
        }

        // TODO - handle other roles and (resource) permissions here 
        
        return AuthorizationErrors.InsufficientPermissions;
    }
}