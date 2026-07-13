using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Context;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Memberships.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Common.Models.Enums;

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
        var scope = _contextProvider.Scope;

        if (scope == Scope.Server)
        {
            // Not handling any server level requests at the moment
            return AuthorizationErrors.InsufficientPermissions;
        }

        var organizationId = _contextProvider.Organization.Id;
        var user = await _userProvider.Get(cancellationToken);

        var memberships = await _membershipRepository.Search(
            new UserMembershipsByOrganizationSpecification(user.Id, organizationId), cancellationToken);

        // Bypass all other authorization logic when the user is the organization owner
        if (memberships.IsOrgOwner(organizationId))
        {
            return Result.Success;
        }

        // Add authorization logic for other roles & permissions here

        return AuthorizationErrors.InsufficientPermissions;
    }
}