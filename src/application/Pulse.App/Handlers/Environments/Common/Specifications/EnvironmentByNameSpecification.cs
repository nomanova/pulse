using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Organizations;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class EnvironmentByNameSpecification(
    OrganizationId organizationId,
    ApplicationId applicationId,
    string environmentName,
    bool includeDeleted = false) : Specification<Environment>
{
    public override Expression<Func<Environment, bool>> ToExpression()
    {
        Expression<Func<Environment, bool>> expression = environment =>
            environment.OrganizationId == organizationId &&
            environment.ApplicationId == applicationId &&
            environment.Name.Value == environmentName;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(environment => !environment.IsDeleted);
        }

        return expression;
    }
}