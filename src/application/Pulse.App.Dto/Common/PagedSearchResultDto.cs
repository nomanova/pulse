namespace Pulse.App.Dto.Common;

public sealed record PagedSearchResultDto<T> : SearchResultDto<T>
{
    public bool HasNext { get; init; }
}