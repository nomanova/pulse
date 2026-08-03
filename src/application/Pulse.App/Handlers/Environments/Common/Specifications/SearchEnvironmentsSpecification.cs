using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Extensions;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;

namespace Pulse.App.Handlers.Environments.Common.Specifications;

public sealed class SearchEnvironmentsSpecification : Specification<Environment>
{
    private readonly ApplicationId _applicationId;
    private readonly string? _query;

    public SearchEnvironmentsSpecification(
        ApplicationId applicationId,
        string? query)
    {
        _applicationId = applicationId;
        _query = query.AsNormalizedQueryable();
    }

    public override Expression<Func<Environment, bool>> ToExpression()
    {
        Expression<Func<Environment, bool>> expr = environment => environment.ApplicationId == _applicationId &&
                                                                  !environment.IsDeleted;

        return expr.WithNameFilter(_query);
    }
}