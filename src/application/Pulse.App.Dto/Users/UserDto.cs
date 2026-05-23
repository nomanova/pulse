namespace Pulse.App.Dto.Users;

public sealed record UserDto
{
    public required string Id { get; init; }

    public required string FirstName { get; init; }
    
    public required string LastName { get; init; }
    
    public string? EmailAddress { get; init; }
}