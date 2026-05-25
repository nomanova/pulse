namespace Pulse.App.Dto.Users;

public sealed record AuthDto
{
    public required string AccessToken { get; init; }
    
    public required UserDto User { get; init; }
}