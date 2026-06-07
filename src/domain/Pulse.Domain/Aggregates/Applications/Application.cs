using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Text;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Applications;

public sealed record ApplicationId : EntityId<ApplicationId, Application>;

public sealed class Application : DomainEntity<ApplicationId>, 
    IOrganizationScoped<OrganizationId>, INamed
{
    public OrganizationId OrganizationId { get; } = null!;

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    private Application()
    {
    }

    private Application(
        ApplicationId id,
        OrganizationId organizationId,
        string name,
        string normalizedName) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        NormalizedName = normalizedName;
    }

    public static Application Create(string? name, Organization organization)
    {
        var nameValue = name.AsName().Assert();
        var id = IdentityProvider.New<ApplicationId>();

        var application = new Application(
            id,
            organization.Id,
            nameValue,
            nameValue.AsNormalizedQueryable());

        application.SetCreated();

        return application;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}