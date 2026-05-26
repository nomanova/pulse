using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Memberships;

public sealed record MembershipId : EntityId<MembershipId, Membership>;

public sealed class Membership : DomainEntity<MembershipId>, IOrganizationScoped
{
    public Scope Scope { get; private set; }

    public OrganizationId OrganizationId { get; } = null!;

    public ApplicationId? ApplicationId { get; private set; }

    public EnvironmentId? EnvironmentId { get; private set; }

    private Membership()
    {
    }

    private Membership(
        MembershipId id,
        Scope scope,
        OrganizationId organizationId,
        ApplicationId? applicationId,
        EnvironmentId? environmentId) : base(id)
    {
        Scope = scope;
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        EnvironmentId = environmentId;
    }

    public static Membership Create(Organization organization)
    {
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Organization, organization.Id, null, null);

        membership.SetCreated();

        return membership;
    }

    public static Membership Create(Application application)
    {
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Application, application.OrganizationId, application.Id, null);

        membership.SetCreated();

        return membership;
    }

    public static Membership Create(Environment environment)
    {
        var id = IdentityProvider.New<MembershipId>();
        var membership = new Membership(
            id, Scope.Environment, environment.OrganizationId, environment.ApplicationId, environment.Id);

        membership.SetCreated();

        return membership;
    }
}