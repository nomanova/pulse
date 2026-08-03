using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Aggregates.WorkflowInstances.Events;

public class WorkflowInstanceStepStartedEvent : IDomainEvent
{
    public WorkflowInstanceId WorkflowInstanceId { get; }
    
    public WorkflowInstanceStepId WorkflowInstanceStepId { get; }
    
    public uint Order { get; }
    
    public WorkflowInstanceStepStartedEvent(
        WorkflowInstanceId workflowInstanceId, 
        WorkflowInstanceStepId workflowInstanceStepId, 
        uint order)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowInstanceStepId = workflowInstanceStepId;
        Order = order;
    }
}