using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.WorkflowInstances;
using Pulse.App.Handlers.WorkflowInstances.Common;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record TriggerWorkflowCommand : ICommand<ErrorOr<WorkflowInstanceDto>>
{
    public required WorkflowId WorkflowId { get; init; }
}

public sealed class TriggerWorkflowCommandAuthorizer : ApiKeyAuthorizer<TriggerWorkflowCommand>;

public sealed class TriggerWorkflowCommandHandler :
    ICommandHandler<TriggerWorkflowCommand, ErrorOr<WorkflowInstanceDto>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TriggerWorkflowCommandHandler(
        IWorkflowRepository workflowRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<WorkflowInstanceDto>> Handle(TriggerWorkflowCommand command,
        CancellationToken cancellationToken)
    {
        // Fetch
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // Trigger
        var instance = workflow.Trigger();

        _workflowInstanceRepository.Add(instance);
        await _unitOfWork.Commit(cancellationToken);

        return instance.ToDto();
    }
}