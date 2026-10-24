using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments.ValueObjects;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Environments;

public sealed record EnvironmentId : EntityId<EnvironmentId, Environment>;

public class Environment : DomainEntity<EnvironmentId>, IApplicationScoped, INamedObject, IKeyedObject
{
    public const string DefaultName = "Production";
    public const string DefaultKey = "production";
    
    public ApplicationId ApplicationId { get; } = null!;

    public ObjectName Name { get; private set; } = null!;

    public ObjectKey Key { get; private set; } = null!;

    public bool IsPrimary { get; private set; }

    public ApiKey ApiKey { get; } = null!;

    private Environment()
    {
    }

    private Environment(
        EnvironmentId id,
        ApplicationId applicationId,
        ObjectName name,
        ObjectKey key,
        bool isPrimary,
        ApiKey apiKey) : base(id)
    {
        ApplicationId = applicationId;
        Name = name;
        Key = key;
        IsPrimary = isPrimary;
        ApiKey = apiKey;
    }

    public static Environment Create(string? name, string? key, Application application, bool isPrimary = false)
    {
        var objectName = ObjectName.Create(name).Assert();
        var objectKey = ObjectKey.Create(key).Assert();
        
        var id = IdentityProvider.New<EnvironmentId>();
        var apiKey = ApiKey.Create();

        var environment = new Environment(
            id,
            application.Id,
            objectName,
            objectKey,
            isPrimary,
            apiKey);

        environment.SetCreated();

        return environment;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value} ({Key.Value})";
    }
}