namespace Pulse.App.Dto.Common;

public record SearchResultDto<T>
{
    public IReadOnlyList<T> Entities { get; init; } = new List<T>();
}