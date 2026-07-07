using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class WorkflowInstance
    {
        public static readonly Error NotRunning =
            Error.Validation("WorkflowInstance.NotRunning", "The workflow instance is not running.");

        public static readonly Error NoRunningStep =
            Error.Validation("WorkflowInstance.NoRunningStep", "The workflow instance does not have a running step.");

        public static readonly Error StepNotPending =
            Error.Validation("WorkflowInstance.StepNotPending", "The workflow instance step is not pending.");

        public static readonly Error StepNotRunning =
            Error.Validation("WorkflowInstance.StepNotRunning", "The workflow instance step is not running.");
    }
}