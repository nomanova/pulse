using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;

namespace Pulse.App.Handlers.Applications.Common.Specifications;

public sealed class ApplicationByIdSpecification(
    OrganizationId organizationId,
    ApplicationId id,
    bool includeDeleted = false) : Specification<Application>
{
    public override Expression<Func<Application, bool>> ToExpression()
    {
        Expression<Func<Application, bool>> expression = application =>
            application.OrganizationId == organizationId &&
            application.Id == id;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(entity => !entity.IsDeleted);
        }

        return expression;
    }
}