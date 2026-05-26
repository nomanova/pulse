using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Roles.Entities;

public sealed record PermissionId : EntityId<PermissionId, Permission>;

public class Permission : Entity<PermissionId>
{
    public RoleId RoleId { get; private set; } = null!;
    
    public string Key { get; private set; } = null!;

    private Permission()
    {
    }
    
    private Permission(
        PermissionId id, 
        RoleId roleId, 
        string key) : base(id)
    {
        RoleId = roleId;
        Key = key;
    }
    
    internal static Permission Create(
        RoleId roleId, 
        string key)
    {
        var id = IdentityProvider.New<PermissionId>();
        return new Permission(id, roleId, key);
    }
    
    public override string ToString()
    {
        return $"[{Id.Value}] {Key}";
    }
}