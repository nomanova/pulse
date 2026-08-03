using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Common.Specifications;

public sealed class WorkflowByNameSpecification(
    EnvironmentId environmentId,
    string? workflowName,
    bool includeDeleted = false) : Specification<Workflow>
{
    public override Expression<Func<Workflow, bool>> ToExpression()
    {
        Expression<Func<Workflow, bool>> expression = workflow => workflow.EnvironmentId == environmentId &&
                                                                  workflow.Name.Value == workflowName;

        if (!includeDeleted)
        {
            expression = expression.AndAlso(workflow => !workflow.IsDeleted);
        }

        return expression;
    }
}