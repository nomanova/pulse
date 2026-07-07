namespace Pulse.Domain.Aggregates.WorkflowInstances.Enums;

public enum WorkflowInstanceStepStatus
{
    Pending,
    Running,
    Completed,
    Skipped,
    Failed,
    Cancelled
}