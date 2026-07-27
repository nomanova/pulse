using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Context;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Workflows.Common;
using Pulse.App.Handlers.Workflows.Common.Specifications;

namespace Pulse.App.Handlers.Workflows.Commands;

public sealed class DeleteWorkflowCommand : IEnvironmentRequest, ICommand<ErrorOr<Success>>
{
    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }

    public string? WorkflowName { get; init; }
}

public sealed class DeleteWorkflowCommandAuthorizer : PermissionAuthorizer<DeleteWorkflowCommand>;

public sealed class DeleteWorkflowCommandHandler : ICommandHandler<DeleteWorkflowCommand, ErrorOr<Success>>
{
    private readonly IContextProvider _contextProvider;
    private readonly IWorkflowRepository _workflowRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWorkflowCommandHandler(
        IContextProvider contextProvider,
        IWorkflowRepository workflowRepository,
        IUnitOfWork unitOfWork)
    {
        _contextProvider = contextProvider;
        _workflowRepository = workflowRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteWorkflowCommand command, CancellationToken cancellationToken)
    {
        var organization = _contextProvider.Organization;
        var application = _contextProvider.Application;
        var environment = _contextProvider.Environment;

        // Fetch workflow
        var specification =
            new WorkflowByNameSpecification(organization.Id, application.Id, environment.Id, command.WorkflowName);
        var workflow = await _workflowRepository.SearchOne(specification, cancellationToken);

        if (workflow == null)
        {
            return Error.NotFound();
        }

        // Delete
        _workflowRepository.Remove(workflow);
        await _unitOfWork.Commit(cancellationToken);

        return Result.Success;
    }
}