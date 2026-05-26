using System.Collections.Generic;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles.Entities;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Models.Text;

namespace Pulse.Domain.Aggregates.Roles;

public sealed record RoleId : EntityId<RoleId, Role>;

public class Role : DomainEntity<RoleId>, INamed
{
    public Source Source { get; private set; }

    // Determines on which level the role can be assigned
    public Scope? Scope { get; private set; }
    
    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    public string? Description { get; private set; }
    
    // Null for a system role, as system roles apply to any organization
    public OrganizationId? OrganizationId { get; private set; }
    
    private readonly List<Permission> _permissions = [];

    public IReadOnlyCollection<Permission> Permission => _permissions.AsReadOnly();

    private Role()
    {
    }

    private Role(
        RoleId id,
        Scope scope,
        Source source,
        OrganizationId organizationId,
        string name,
        string normalizedName,
        string? description) : base(id)
    {
        Scope = scope;
        Source = source;
        OrganizationId = organizationId;
        Name = name;
        NormalizedName = normalizedName;
        Description = description;
    }
    
    /**
     * System | Organization | Admin
     *  These users have full control over the entire system and all application within it.
     * System | Organization | Manager
     *  These users can create new applications and use them. They can also assign other users permissions to their applications.
     * System | Organization | User
     *  Login-access only.
     *
     * System | Application | Owner
     *  These users have full control over the application and all its environments.
     * System | Application | Member
     *  These users can view most application level details and create new environments.
     *
     * System | Environment | Owner
     * System | Environment | Member
     * System | Environment | Viewer
     */
    public static Role CreateSystem(RoleId roleId, Scope scope, string? name, string? description, Organization organization)
    {
        var nameValue = name.AsName().Assert();
        var descriptionValue = description.AsOptionalText(nameof(Description)).Assert();
        
        var role = new Role(
            roleId,
            scope,
            Source.System,
            organization.Id,
            nameValue,
            nameValue.AsNormalizedQueryable(),
            descriptionValue);

        role.SetCreated();

        return role;
    }

    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}