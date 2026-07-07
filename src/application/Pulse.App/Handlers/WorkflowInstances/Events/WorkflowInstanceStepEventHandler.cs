using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pulse.App.Common.Database;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.WorkflowInstances.Common;
using Pulse.App.Handlers.WorkflowInstances.Common.Specifications;
using Pulse.Domain.Aggregates.WorkflowInstances.Events;

namespace Pulse.App.Handlers.WorkflowInstances.Events;

public sealed class WorkflowInstanceStepEventHandler
    : IdempotentNotificationHandler<WorkflowInstanceStepStartedEvent>
{
    private readonly ILogger<WorkflowInstanceStepEventHandler> _logger;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWorkflowStepExecutor _workflowStepExecutor;

    public WorkflowInstanceStepEventHandler(
        ILogger<WorkflowInstanceStepEventHandler> logger,
        IInbox inbox,
        INotificationContext notificationContext,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IUnitOfWork unitOfWork,
        IWorkflowStepExecutor workflowStepExecutor)
        : base(inbox, notificationContext)
    {
        _logger = logger;
        _workflowInstanceRepository = workflowInstanceRepository;
        _unitOfWork = unitOfWork;
        _workflowStepExecutor = workflowStepExecutor;
    }

    protected override async Task HandleIdempotently(
        WorkflowInstanceStepStartedEvent notification,
        CancellationToken cancellationToken)
    {
        var workflowInstance =
            await _workflowInstanceRepository.SearchOne(
                new WorkflowInstanceByIdSpecification(notification.WorkflowInstanceId), cancellationToken);

        if (workflowInstance is null)
        {
            _logger.LogError("Workflow instance not found: {WorkflowInstanceId}",
                notification.WorkflowInstanceId);
            return;
        }

        try
        {
            await _workflowStepExecutor.Execute(
                notification.WorkflowInstanceId,
                notification.WorkflowInstanceStepId,
                notification.WorkflowVersionStepId,
                cancellationToken);

            workflowInstance.CompleteStep(notification.WorkflowInstanceStepId);
        }
        catch
        {
            workflowInstance.FailStep(notification.WorkflowInstanceStepId);
            throw;
        }
        finally
        {
            _workflowInstanceRepository.Update(workflowInstance);
            await _unitOfWork.Commit(cancellationToken);
        }
    }
}