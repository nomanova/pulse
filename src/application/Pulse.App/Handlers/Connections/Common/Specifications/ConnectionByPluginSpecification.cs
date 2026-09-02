using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Connections;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Handlers.Connections.Common.Specifications;

public sealed class ConnectionByPluginSpecification(
    EnvironmentId environmentId, string pluginId, bool includeDeleted = false) : Specification<Connection>
{
    public override Expression<Func<Connection, bool>> ToExpression()
    {
        Expression<Func<Connection, bool>> expression = connection => 
            connection.EnvironmentId == environmentId && connection.PluginId == pluginId;
        
        if (!includeDeleted)
        {
            expression = expression.AndAlso(connection => !connection.IsDeleted);
        }
        
        return expression;
    }
}