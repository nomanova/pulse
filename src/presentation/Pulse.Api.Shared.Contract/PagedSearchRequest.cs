namespace Pulse.Api.Shared.Contract;

public record PagedSearchRequest
{
    public string? LastId { get; init; }

    public uint? PageSize { get; init; }
    
    public bool? Ascending { get; init; }
}

public record NamedPagedSearchRequest : PagedSearchRequest
{
    public string? Query { get; init; }
    
    public string? OrderBy { get; init; }
}