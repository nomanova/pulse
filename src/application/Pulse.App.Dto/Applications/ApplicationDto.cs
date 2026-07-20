namespace Pulse.App.Dto.Applications;

public sealed record ApplicationDto
{
    public required string Id { get; set; }

    public required string Name { get; set; }
}