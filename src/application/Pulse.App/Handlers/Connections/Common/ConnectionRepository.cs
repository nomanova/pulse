using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Connections;

namespace Pulse.App.Handlers.Connections.Common;

public interface IConnectionRepository : IRepository<Connection>;

internal sealed class ConnectionRepository : Repository<Connection>, IConnectionRepository
{
    public ConnectionRepository(IDatabaseContext context) : base(context.Connections)
    {
    }
}