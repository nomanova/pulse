using System.Collections.Generic;
using System.Linq;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public static class WorkflowVersionExtensions
{
    public static uint NextVersionNumber(this List<WorkflowVersion> versions)
    {
        return versions.Count == 0
            ? 1
            : versions.Max(version => version.Version) + 1;
    }
}