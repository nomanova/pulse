using System.Collections.Generic;
using System.Linq;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

internal static class WorkflowVersionStepExtensions
{
    extension(IReadOnlyCollection<WorkflowVersionStep> steps)
    {
        internal WorkflowVersionStep? Find(WorkflowVersionStepId stepId)
        {
            return steps.FirstOrDefault(step => step.Id == stepId);
        }

        internal uint NextOrder()
        {
            return steps.Count == 0
                ? 1
                : steps.Max(step => step.Order) + 1;
        }

        internal List<WorkflowVersionStep> Reorder()
        {
            var orderedSteps = steps
                .OrderBy(step => step.Order)
                .ToList();

            var reorderedSteps = new List<WorkflowVersionStep>();

            for (var index = 0; index < orderedSteps.Count; index++)
            {
                var step = orderedSteps[index];
                step.SetOrder((uint)index + 1);
                reorderedSteps.Add(step);
            }

            return reorderedSteps;
        }
    }
}