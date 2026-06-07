using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Text;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Environments;

public sealed record EnvironmentId : EntityId<EnvironmentId, Environment>;

public class Environment : DomainEntity<EnvironmentId>, 
    IOrganizationScoped<OrganizationId>, IApplicationScoped, INamed
{
    public OrganizationId OrganizationId { get; } = null!;

    public ApplicationId ApplicationId { get; } = null!;

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    private Environment()
    {
    }

    private Environment(
        EnvironmentId id,
        OrganizationId organizationId,
        ApplicationId applicationId,
        string name,
        string normalizedName) : base(id)
    {
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        Name = name;
        NormalizedName = normalizedName;
    }

    public static Environment Create(string? name, Application application)
    {
        var nameValue = name.AsName().Assert();
        var id = IdentityProvider.New<EnvironmentId>();

        var environment = new Environment(
            id,
            application.OrganizationId,
            application.Id,
            nameValue,
            nameValue.AsNormalizedQueryable());

        environment.SetCreated();

        return environment;
    }
}