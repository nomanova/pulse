using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Text;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Organizations;

public sealed record OrganizationId : EntityId<OrganizationId, Organization>;

public sealed class Organization : DomainEntity<OrganizationId>, INamed
{
    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    private Organization()
    {
    }

    private Organization(
        OrganizationId id,
        string name,
        string normalizedName) : base(id)
    {
        Name = name;
        NormalizedName = normalizedName;
    }

    public static Organization Create(string? name)
    {
        var nameValue = name.AsName().Assert();
        var id = IdentityProvider.New<OrganizationId>();

        var organization = new Organization(
            id,
            nameValue,
            nameValue.AsNormalizedQueryable());

        organization.SetCreated();

        return organization;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}