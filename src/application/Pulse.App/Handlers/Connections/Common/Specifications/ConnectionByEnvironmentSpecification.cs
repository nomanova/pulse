using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Channels;

namespace Pulse.App.Handlers.Connections.Common.Specifications;

public sealed class ConnectionByEnvironmentSpecification(
    EnvironmentId environmentId, Channel? channel, bool includeDeleted = false) : Specification<Connection>
{
    public override Expression<Func<Connection, bool>> ToExpression()
    {
        Expression<Func<Connection, bool>> expression = connection => connection.EnvironmentId == environmentId;

        if (channel != null)
        {
            expression = expression.AndAlso(connection => connection.Channel == channel);
        }
        
        if (!includeDeleted)
        {
            expression = expression.AndAlso(connection => !connection.IsDeleted);
        }
        
        return expression;
    }
}