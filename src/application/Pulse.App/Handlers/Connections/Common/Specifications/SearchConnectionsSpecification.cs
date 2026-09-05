using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Connections.Common.Specifications;

public sealed class SearchConnectionsSpecification : Specification<Connection>
{
    private readonly EnvironmentId _environmentId;

    public SearchConnectionsSpecification(EnvironmentId environmentId)
    {
        _environmentId = environmentId;
    }

    public override Expression<Func<Connection, bool>> ToExpression()
    {
        return connection => connection.EnvironmentId == _environmentId;
    }
}