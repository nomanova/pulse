using System;
using System.Linq.Expressions;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Aggregates.Workflows.Enums;

namespace Pulse.App.Handlers.Workflows.Common.Specifications;

public sealed class SearchWorkflowVersionsSpecification : Specification<WorkflowVersion>
{
    private readonly WorkflowId _workflowId;
    private readonly WorkflowVersionStatus? _status;

    public SearchWorkflowVersionsSpecification(
        WorkflowId workflowId, WorkflowVersionStatus? status)
    {
        _workflowId = workflowId;
        _status = status;
    }

    public override Expression<Func<WorkflowVersion, bool>> ToExpression()
    {
        Expression<Func<WorkflowVersion, bool>> expression = workflowVersion => 
            workflowVersion.WorkflowId == _workflowId;

        if (_status != null)
        {
            expression = expression.AndAlso(workflowVersion => workflowVersion.Status == _status);
        }

        return expression;
    }
}