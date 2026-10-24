using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class EnvironmentByKeySpecification(
    ApplicationId applicationId,
    string? environmentKey,
    bool includeDeleted = false) : Specification<Environment>
{
    public override Expression<Func<Environment, bool>> ToExpression()
    {
        Expression<Func<Environment, bool>> expression = environment =>
            environment.ApplicationId == applicationId &&
            environment.Key.Value == environmentKey;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(environment => !environment.IsDeleted);
        }

        return expression;
    }
}