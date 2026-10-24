using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Applications.Common.Specifications;

public sealed class ApplicationByKeySpecification(
    OrganizationId organizationId,
    string? applicationKey,
    bool includeDeleted = false) : Specification<Application>
{
    public override Expression<Func<Application, bool>> ToExpression()
    {
        Expression<Func<Application, bool>> expression = application =>
            application.OrganizationId == organizationId &&
            application.Key.Value == applicationKey;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(application => !application.IsDeleted);
        }

        return expression;
    }
}