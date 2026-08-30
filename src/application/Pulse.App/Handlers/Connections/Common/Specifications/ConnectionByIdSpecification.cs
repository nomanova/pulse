using Pulse.App.Common.Database.Specifications;
using Pulse.Domain.Aggregates.Connections;

namespace Pulse.App.Handlers.Connections.Common.Specifications;

public sealed class ConnectionByIdSpecification(ConnectionId id, bool includeDeleted = false) : 
    ByIdSpecification<Connection, ConnectionId>(id, includeDeleted);