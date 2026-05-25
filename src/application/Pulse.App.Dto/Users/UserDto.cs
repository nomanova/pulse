namespace Pulse.App.Dto.Users;

public sealed record UserDto
{
    public required string Id { get; init; }

    public required string Username { get; init; }
}