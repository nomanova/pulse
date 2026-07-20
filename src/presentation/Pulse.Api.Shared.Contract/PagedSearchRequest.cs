namespace Pulse.Api.Shared.Contract;

public record PagedSearchRequest
{
    public string? Query { get; init; }

    public string? LastId { get; init; }

    public uint? PageSize { get; init; }
    
    public bool? Ascending { get; init; }
    
    public string? OrderBy { get; init; }
}