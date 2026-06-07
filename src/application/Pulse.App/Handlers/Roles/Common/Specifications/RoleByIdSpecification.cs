using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Roles.Common.Specifications;

public sealed class RoleByIdSpecification(
    RoleId id,
    bool includeDeleted = false) : ByIdSpecification<Role, RoleId>(id, includeDeleted);