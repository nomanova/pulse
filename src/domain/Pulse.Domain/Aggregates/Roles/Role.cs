using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.Domain.Aggregates.Roles;

public sealed record RoleId : EntityId<RoleId, Role>;

public partial class Role : DomainEntity<RoleId>, INamed
{
    public bool IsBuiltIn { get; private set; }

    public Scope Scope { get; private set; }

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    private Role()
    {
    }

    private Role(
        RoleId id,
        bool isBuiltIn,
        Scope scope,
        string name) : base(id)
    {
        IsBuiltIn = isBuiltIn;
        Scope = scope;
        Name = name;
        NormalizedName = name.AsNormalizedQueryable();
        
        SetCreated();
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}