namespace Pulse.Web.Core.Models;

public sealed record UserProfile
{
    public required string Id { get; init; }
    
    public required string Username { get; init; }
}