using Pulse.Domain.Aggregates.Applications.Events;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Applications;

public sealed record ApplicationId : EntityId<ApplicationId, Application>;

public sealed class Application : DomainEntity<ApplicationId>,
    IOrganizationScoped, INamedObject, IKeyedObject
{
    public OrganizationId OrganizationId { get; } = null!;

    public ObjectName Name { get; private set; } = null!;

    public ObjectKey Key { get; private set; } = null!;

    private Application()
    {
    }

    private Application(
        ApplicationId id,
        OrganizationId organizationId,
        ObjectName name,
        ObjectKey key) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
        Key = key;
    }

    public static Application Create(string? name, string? key, Organization organization)
    {
        var objectName = ObjectName.Create(name).Assert();
        var objectKey = ObjectKey.Create(key).Assert();
        
        var id = IdentityProvider.New<ApplicationId>();

        var application = new Application(
            id,
            organization.Id,
            objectName,
            objectKey);

        application.SetCreated();
        application.AddEvent(new ApplicationCreatedEvent(organization.Id, id));

        return application;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value} ({Key.Value})";
    }
}