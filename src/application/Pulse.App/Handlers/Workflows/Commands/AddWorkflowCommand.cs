using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Common.Mappers;
using Pulse.App.Dto.Common;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Workflows;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed record AddWorkflowCommand : ICommand<ErrorOr<IdentityDto>>
{
    public required EnvironmentId EnvironmentId { get; init; }
    
    public string? WorkflowName { get; init; }
}

public sealed class AddWorkflowCommandAuthorizer : ApiKeyAuthorizer<AddWorkflowCommand>;

public sealed class AddWorkflowCommandHandler : ICommandHandler<AddWorkflowCommand, ErrorOr<IdentityDto>>
{
    private readonly IEnvironmentRepository _environmentRepository;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkflowCommandHandler(
        IEnvironmentRepository environmentRepository,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _environmentRepository = environmentRepository;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<IdentityDto>> Handle(AddWorkflowCommand command, CancellationToken cancellationToken)
    {
        // Fetch environment
        var environmentSpecification = new EnvironmentByIdSpecification(command.EnvironmentId);
        var environment = await _environmentRepository.SearchOne(environmentSpecification, cancellationToken);

        if (environment == null)
        {
            return Error.NotFound();
        }
        
        // Duplicate name detection
        var specification = new WorkflowByNameSpecification(environment.Id, command.WorkflowName);
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