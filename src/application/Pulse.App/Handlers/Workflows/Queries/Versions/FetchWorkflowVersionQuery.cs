using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Workflows;

namespace Pulse.App.Handlers.Workflows.Queries.Versions;

public sealed record FetchWorkflowVersionQuery : IQuery<ErrorOr<WorkflowVersionDto>>
{
    public string? WorkflowName { get; init; }
}