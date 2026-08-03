using Pulse.Api.Shared.Contract;

namespace Pulse.Api.Ctrl.Contract;

public sealed record AddWorkflowRequest
{
    public string? EnvironmentId { get; init; }

    public string? WorkflowName { get; init; }
}

public sealed record RemoveWorkflowRequest
{
    public string? WorkflowId { get; init; }
}

public sealed record FetchWorkflowRequest
{
    public string? WorkflowId { get; init; }
}

public sealed record SearchWorkflowsRequest : PagedSearchRequest
{
    public string? EnvironmentId { get; init; }
}

public sealed record AddWorkflowVersionStepRequest
{
    public string? WorkflowId { get; init; }

    public string? WorkflowVersionId { get; init; }
}

public sealed record RemoveWorkflowVersionStepRequest
{
    public string? WorkflowId { get; init; }

    public string? WorkflowVersionId { get; init; }

    public string? WorkflowVersionStepId { get; init; }
}