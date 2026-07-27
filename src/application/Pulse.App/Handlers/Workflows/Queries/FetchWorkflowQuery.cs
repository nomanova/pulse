using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;

namespace Pulse.App.Handlers.Workflows.Queries;

public sealed record FetchWorkflowQuery : IEnvironmentRequest, IQuery<ErrorOr<WorkflowDto>>
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }

    public string? WorkflowName { get; init; }
}

public sealed class FetchWorkflowQueryAuthorizer : PermissionAuthorizer<FetchWorkflowQuery>;

public sealed class FetchWorkflowQueryHandler : IQueryHandler<FetchWorkflowQuery, ErrorOr<WorkflowDto>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IWorkflowRepository _workflowRepository;

    public FetchWorkflowQueryHandler(
        IContextProvider contextProvider,
        IWorkflowRepository workflowRepository)
    {
        _contextProvider = contextProvider;
        _workflowRepository = workflowRepository;
    }

    public async Task<ErrorOr<WorkflowDto>> Handle(FetchWorkflowQuery query, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;
        var environment = _contextProvider.Environment;

        if (string.IsNullOrEmpty(query.WorkflowName))
        {
            return Error.NotFound();
        }

        // Fetch workflow
        var specification =
            new WorkflowByNameSpecification(organization.Id, application.Id, environment.Id, query.WorkflowName);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        return workflow.ToDto();
    }
}