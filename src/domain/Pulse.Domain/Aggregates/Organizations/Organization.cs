using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Organizations;

public sealed record OrganizationId : EntityId<OrganizationId, Organization>;

public sealed class Organization : DomainEntity<OrganizationId>, INamedObject
{
    public ObjectName Name { get; private set; } = null!;

    private Organization()
    {
    }

    private Organization(
        OrganizationId id,
        ObjectName name) : base(id)
    {
        Name = name;
    }

    public static Organization Create(string? name)
    {
        var objectName = ObjectName.Create(name).Assert();
        var id = IdentityProvider.New<OrganizationId>();

        var organization = new Organization(id, objectName);
        organization.SetCreated();

        return organization;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value}";
    }
}