using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Common.Extensions;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;

namespace Pulse.App.Handlers.Workflows.Common.Specifications;

public sealed class SearchWorkflowsSpecification : Specification<Workflow>
{
    private readonly EnvironmentId _environmentId;
    private readonly string? _query;

    public SearchWorkflowsSpecification(
        EnvironmentId environmentId,
        string? query)
    {
        _environmentId = environmentId;
        _query = query.AsNormalizedQueryable();
    }

    public override Expression<Func<Workflow, bool>> ToExpression()
    {
        Expression<Func<Workflow, bool>> expr = workflow => workflow.EnvironmentId == _environmentId &&
                                                            !workflow.IsDeleted;

        return expr.WithNameFilter(_query);
    }
}