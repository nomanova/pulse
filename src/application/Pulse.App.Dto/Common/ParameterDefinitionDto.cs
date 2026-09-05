namespace Pulse.App.Dto.Common;

public record ParameterDefinitionDto
{
    public required string Key { get; init; }
    
    public required string Name { get; init; }
    
    public string? Description { get; init; }
    
    public bool IsRequired { get; init; }
}