using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Memberships;

public sealed record MembershipId : EntityId<MembershipId, Membership>;

public sealed class Membership : DomainEntity<MembershipId>, IOrganizationScoped
{
    public Scope Scope { get; private set; }

    public UserId UserId { get; private set; } = null!;
    
    public RoleId RoleId { get; private set; } = null!;
    
    public OrganizationId OrganizationId { get; } = null!;
    
    public ApplicationId? ApplicationId { get; private set; }

    public EnvironmentId? EnvironmentId { get; private set; }
    
    private Membership()
    {
    }

    private Membership(
        MembershipId id,
        Scope scope,
        UserId userId,
        RoleId roleId,
        OrganizationId organizationId,
        ApplicationId? applicationId,
        EnvironmentId? environmentId) : base(id)
    {
        Scope = scope;
        UserId = userId;
        RoleId = roleId;
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        EnvironmentId = environmentId;
    }

    public static Membership Create(User user, Role role, Organization organization)
    {
        DomainErrors.Membership.InvalidRoleScope.Assert(() => role.Scope == Scope.Organization);
        
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Organization, user.Id, role.Id, organization.Id, null, null);

        membership.SetCreated();

        return membership;
    }

    public static Membership Create(User user, Role role, Application application)
    {
        DomainErrors.Membership.InvalidRoleScope.Assert(() => role.Scope == Scope.Application);
        
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Application, user.Id, role.Id, application.OrganizationId, application.Id, null);

        membership.SetCreated();

        return membership;
    }

    public static Membership Create(User user, Role role, Environment environment)
    {
        DomainErrors.Membership.InvalidRoleScope.Assert(() => role.Scope == Scope.Environment);
        
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Environment, user.Id, role.Id, environment.OrganizationId, environment.ApplicationId, environment.Id);

        membership.SetCreated();

        return membership;
    }
}