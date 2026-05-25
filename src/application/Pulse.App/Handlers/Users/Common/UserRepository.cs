using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Handlers.Users.Common;

public interface IUserRepository : IRepository<User>;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(IDatabaseContext context) : base(context.Users)
    {
    }
}