using System.Text.Json.Serialization;
using Pulse.Domain.Aggregates.WorkflowInstances.Entities;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Aggregates.WorkflowInstances.Events;

public class WorkflowInstanceStepStartedEvent : IDomainEvent
{
    [JsonInclude]
    public WorkflowInstanceId WorkflowInstanceId { get; private set; }
    
    [JsonInclude]
    public WorkflowInstanceStepId WorkflowInstanceStepId { get; private set; }
    
    [JsonInclude]
    public uint Order { get; private set; }
    
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