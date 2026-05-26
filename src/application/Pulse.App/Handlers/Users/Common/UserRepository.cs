using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Handlers.Users.Common;

public interface IUserRepository : IRepository<User>;

internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDatabaseContext context) : base(context.Users)
    {
    }
}