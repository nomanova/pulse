using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Mappers;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record CreateWorkflowCommand : IEnvironmentRequest, ICommand<ErrorOr<IdentityDto>>
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }

    public string? WorkflowName { get; init; }
}

public sealed class CreateWorkflowCommandAuthorizer : PermissionAuthorizer<CreateWorkflowCommand>;

public sealed class CreateWorkflowCommandHandler : ICommandHandler<CreateWorkflowCommand, ErrorOr<IdentityDto>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkflowCommandHandler(
        IContextProvider contextProvider,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _contextProvider = contextProvider;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(CreateWorkflowCommand command, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;
        var environment = _contextProvider.Environment;

        // Duplicate name detection
        var specification =
            new WorkflowByNameSpecification(organization.Id, application.Id, environment.Id, command.WorkflowName);
        var existingWorkflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (existingWorkflow != null)
        {
            return ApplicationErrors.NameInUse;
        }

        // Create workflow
        var workflow = Workflow.Create(environment, command.WorkflowName);
        _workflowRepository.Add(workflow);

        await _unitOfWork.Commit(cancellationToken);

        return workflow.ToIdentityDto();
    }
}