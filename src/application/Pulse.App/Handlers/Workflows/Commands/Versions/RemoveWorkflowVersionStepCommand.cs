using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Handlers.Workflows.Commands.Versions;

public sealed record RemoveWorkflowVersionStepCommand : ICommand<ErrorOr<Success>>
{
    public required WorkflowId WorkflowId { get; init; }

    public required WorkflowVersionId VersionId { get; init; }

    public required WorkflowVersionStepId StepId { get; init; }
}

public sealed class RemoveWorkflowVersionStepCommandAuthorizer : PermissionAuthorizer<RemoveWorkflowVersionStepCommand>;

public sealed class RemoveWorkflowVersionStepCommandHandler :
    ICommandHandler<RemoveWorkflowVersionStepCommand, ErrorOr<Success>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWorkflowVersionStepCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveWorkflowVersionStepCommand command,
        CancellationToken cancellationToken)
    {
        // Fetch
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        var workflowVersion = workflow.Versions.Find(command.VersionId);

        if (workflowVersion == null)
        {
            return Error.NotFound();
        }

        // Remove
        workflowVersion.RemoveStep(command.StepId);

        _workflowRepository.Update(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}