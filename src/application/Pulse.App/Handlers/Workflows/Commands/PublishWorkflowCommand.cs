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

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record PublishWorkflowCommand : ICommand<ErrorOr<WorkflowVersionDto>>
{
    public required WorkflowId WorkflowId { get; init; }
}

public sealed class PublishWorkflowCommandAuthorizer : ApiKeyAuthorizer<PublishWorkflowCommand>;

public sealed class PublishWorkflowCommandHandler :
    ICommandHandler<PublishWorkflowCommand, ErrorOr<WorkflowVersionDto>>
{
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishWorkflowCommandHandler(
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<WorkflowVersionDto>> Handle(PublishWorkflowCommand command,
        CancellationToken cancellationToken)
    {
        // Fetch
        var specification = new WorkflowByIdSpecification(command.WorkflowId);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // Publish
        var publishedVersion = workflow.PublishDraftVersion();

        _workflowRepository.Update(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return publishedVersion.ToDto();
    }
}