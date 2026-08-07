using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Workflows;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Commands.Versions;

public sealed record AddWorkflowVersionStepCommand : ICommand<ErrorOr<WorkflowVersionStepDto>>
{
    public required WorkflowId WorkflowId { get; init; }
    
    public required WorkflowVersionId VersionId { get; init; }
}

public sealed class AddWorkflowVersionStepCommandAuthorizer : PermissionAuthorizer<AddWorkflowVersionStepCommand>;

public sealed class AddWorkflowVersionStepCommandHandler : 
    ICommandHandler<AddWorkflowVersionStepCommand, ErrorOr<WorkflowVersionStepDto>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkflowVersionStepCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<WorkflowVersionStepDto>> Handle(AddWorkflowVersionStepCommand command,
        CancellationToken cancellationToken)
    {
        // Fetch
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // The workflow version is strictly speaking not required, as a step can
        // only be added to the (single) version currently in draft.
        // However, having the client explicitly provide the version will avoid race conditions.
        var workflowVersion = workflow.Versions.Find(command.VersionId);

        if (workflowVersion == null)
        {
            return Error.NotFound();
        }

        // Add
        var step = workflowVersion.AddStep(); // This will trip when the version is no longer in draft.
        
        _workflowRepository.Update(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return step.ToDto();
    }
}