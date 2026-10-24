using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Models.ValueObjects;

namespace Pulse.Domain.Aggregates.Roles;

public sealed record RoleId : EntityId<RoleId, Role>;

public partial class Role : DomainEntity<RoleId>, INamedObject, IKeyedObject
{
    public bool IsBuiltIn { get; private set; }

    public Scope Scope { get; private set; }

    public ObjectName Name { get; private set; } = null!;
    
    public ObjectKey Key { get; private set; } = null!;

    private Role()
    {
    }

    private Role(
        RoleId id,
        bool isBuiltIn,
        Scope scope,
        ObjectName name,
        ObjectKey key) : base(id)
    {
        IsBuiltIn = isBuiltIn;
        Scope = scope;
        Name = name;
        Key = key;
        
        SetCreated();
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name.Value} ({Key.Value})";
    }
}