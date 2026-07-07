using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Aggregates.WorkflowInstances.Events;

public class WorkflowInstanceStepStartedEvent : IDomainEvent
{
    public WorkflowInstanceId WorkflowInstanceId { get; }
    
    public WorkflowInstanceStepId WorkflowInstanceStepId { get; }
    
    public WorkflowId WorkflowId { get; }
    
    public WorkflowVersionId WorkflowVersionId { get; }
    
    public WorkflowVersionStepId WorkflowVersionStepId { get; }
    
    public uint Order { get; }
    
    public WorkflowInstanceStepStartedEvent(
        WorkflowInstanceId workflowInstanceId, 
        WorkflowInstanceStepId workflowInstanceStepId, 
        WorkflowId workflowId, 
        WorkflowVersionId workflowVersionId, 
        WorkflowVersionStepId workflowVersionStepId, 
        uint order)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowInstanceStepId = workflowInstanceStepId;
        WorkflowId = workflowId;
        WorkflowVersionId = workflowVersionId;
        WorkflowVersionStepId = workflowVersionStepId;
        Order = order;
    }
}