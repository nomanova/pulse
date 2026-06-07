using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.App.Handlers.Roles.Common;

public interface IRoleRepository : IRepository<Role>;

internal sealed class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(IDatabaseContext context) : base(context.Roles)
    {
    }
}