using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments.ValueObjects;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Environments;

public sealed record EnvironmentId : EntityId<EnvironmentId, Environment>;

public class Environment : DomainEntity<EnvironmentId>, IApplicationScoped, INamedObject
{
    public const string DefaultName = "production";
    
    public ApplicationId ApplicationId { get; } = null!;

    public ObjectName Name { get; private set; } = null!;

    public bool IsPrimary { get; private set; }

    public ApiKey ApiKey { get; } = null!;

    private Environment()
    {
    }

    private Environment(
        EnvironmentId id,
        ApplicationId applicationId,
        ObjectName name,
        bool isPrimary,
        ApiKey apiKey) : base(id)
    {
        ApplicationId = applicationId;
        Name = name;
        IsPrimary = isPrimary;
        ApiKey = apiKey;
    }

    public static Environment Create(string? name, Application application, bool isPrimary = false)
    {
        var objectName = ObjectName.Create(name).Assert();
        var id = IdentityProvider.New<EnvironmentId>();
        var apiKey = ApiKey.Create();

        var environment = new Environment(
            id,
            application.Id,
            objectName,
            isPrimary,
            apiKey);

        environment.SetCreated();

        return environment;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value}";
    }
}