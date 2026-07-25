using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class EnvironmentByIdSpecification(
    OrganizationId organizationId, 
    ApplicationId applicationId, 
    EnvironmentId id, 
    bool includeDeleted = false) : Specification<Environment>
{
    public override Expression<Func<Environment, bool>> ToExpression()
    {
        Expression<Func<Environment, bool>> expression = environment =>
            environment.OrganizationId == organizationId &&
            environment.ApplicationId == applicationId &&
            environment.Id == id;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}