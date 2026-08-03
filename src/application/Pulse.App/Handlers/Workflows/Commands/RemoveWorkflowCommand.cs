using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record RemoveWorkflowCommand : ICommand<ErrorOr<Success>>
{
    public required WorkflowId WorkflowId { get; init; }
}

public sealed class RemoveWorkflowCommandAuthorizer : PermissionAuthorizer<RemoveWorkflowCommand>;

public sealed class RemoveWorkflowCommandHandler : ICommandHandler<RemoveWorkflowCommand, ErrorOr<Success>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWorkflowCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveWorkflowCommand command, CancellationToken cancellationToken)
    {
        // Fetch workflow
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // Remove
        _workflowRepository.Remove(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}