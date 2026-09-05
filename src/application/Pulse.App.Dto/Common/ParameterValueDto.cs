namespace Pulse.App.Dto.Common;

public sealed record ParameterValueDto
{
    public required string Key { get; init; }

    public required string Value { get; init; }
}