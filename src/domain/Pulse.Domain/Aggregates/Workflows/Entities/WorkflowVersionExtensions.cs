using System.Collections.Generic;
using System.Linq;

namespace Pulse.Domain.Aggregates.Workflows.Entities;

public static class WorkflowVersionExtensions
{
    extension(IReadOnlyCollection<WorkflowVersion> versions)
    {
        public WorkflowVersion? Find(WorkflowVersionId versionId)
        {
            return versions.FirstOrDefault(version => version.Id == versionId);
        }

        public uint NextVersionNumber()
        {
            return versions.Count == 0
                ? 1
                : versions.Max(version => version.Version) + 1;
        }
    }
}