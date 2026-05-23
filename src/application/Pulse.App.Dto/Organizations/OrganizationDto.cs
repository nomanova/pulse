namespace Pulse.App.Dto.Organizations;

public sealed record OrganizationDto
{
    public required string Id { get; init; }
    
    public required string Name { get; init; }
}