using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Environments;

public sealed record EnvironmentId : EntityId<EnvironmentId, Environment>;

public class Environment : DomainEntity<EnvironmentId>, IApplicationScoped, INamedObject
{
    public OrganizationId OrganizationId { get; } = null!;

    public ApplicationId ApplicationId { get; } = null!;

    public ObjectName Name { get; private set; } = null!;

    private Environment()
    {
    }

    private Environment(
        EnvironmentId id,
        OrganizationId organizationId,
        ApplicationId applicationId,
        ObjectName name) : base(id)
    {
        OrganizationId = organizationId;
        ApplicationId = applicationId;
        Name = name;
    }

    public static Environment Create(string? name, Application application)
    {
        var objectName = ObjectName.Create(name).Assert();
        var id = IdentityProvider.New<EnvironmentId>();

        var environment = new Environment(
            id,
            application.OrganizationId,
            application.Id,
            objectName);

        environment.SetCreated();

        return environment;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value}";
    }
}