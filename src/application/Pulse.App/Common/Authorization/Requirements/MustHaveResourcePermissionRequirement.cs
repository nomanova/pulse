using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Memberships.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;

namespace Pulse.App.Common.Authorization.Requirements;

public sealed class MustHaveResourcePermissionRequirement : IAuthorizationRequirement;

public sealed class MustHaveResourcePermissionRequirementHandler :
    IAuthorizationHandler<MustHaveResourcePermissionRequirement>
{
    private readonly ICachedUserProvider _cachedUserProvider;
    private readonly IResourceAuthorizationContextProvider _resourceContextProvider;
    private readonly IMembershipRepository _membershipRepository;

    public MustHaveResourcePermissionRequirementHandler(
        ICachedUserProvider cachedUserProvider,
        IResourceAuthorizationContextProvider resourceContextProvider,
        IMembershipRepository membershipRepository)
    {
        _cachedUserProvider = cachedUserProvider;
        _resourceContextProvider = resourceContextProvider;
        _membershipRepository = membershipRepository;
    }

    public async Task<ErrorOr<Success>> Handle(MustHaveResourcePermissionRequirement request,
        CancellationToken cancellationToken)
    {
        var context = _resourceContextProvider.Context;
        var user = await _cachedUserProvider.Get(cancellationToken);

        var memberships = await _membershipRepository.Search(
            new UserMembershipsByOrganizationSpecification(user.Id, context.OrganizationId), cancellationToken);

        // Bypass all other authorization logic when the user is the organization owner
        if (memberships.IsOrgOwner(context.OrganizationId))
        {
            return Result.Success;
        }
        
        // TODO - add authorization logic for other roles & permissions
        
        return AuthorizationErrors.InsufficientPermissions;
    }
}