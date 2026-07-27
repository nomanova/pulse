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
    private readonly OrganizationId _organizationId;
    private readonly ApplicationId _applicationId;
    private readonly EnvironmentId _environmentId;
    private readonly string? _query;

    public SearchWorkflowsSpecification(
        OrganizationId organizationId,
        ApplicationId applicationId,
        EnvironmentId environmentId,
        string? query)
    {
        _organizationId = organizationId;
        _applicationId = applicationId;
        _environmentId = environmentId;
        _query = query.AsNormalizedQueryable();
    }

    public override Expression<Func<Workflow, bool>> ToExpression()
    {
        Expression<Func<Workflow, bool>> expr = workflow => workflow.OrganizationId == _organizationId &&
                                                            workflow.ApplicationId == _applicationId &&
                                                            workflow.EnvironmentId == _environmentId &&
                                                            !workflow.IsDeleted;

        return expr.WithNameFilter(_query);
    }
}