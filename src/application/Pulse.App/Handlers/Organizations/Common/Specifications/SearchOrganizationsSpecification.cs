using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common.Specifications;

public sealed class SearchOrganizationsSpecification : Specification<Organization>
{
    private readonly string? _query;

    public SearchOrganizationsSpecification(string? query)
    {
        _query = query;
    }

    public override Expression<Func<Organization, bool>> ToExpression()
    {
        Expression<Func<Organization, bool>> expr = organization => !organization.IsDeleted;
        return expr.WithNameFilter(_query);
    }
}