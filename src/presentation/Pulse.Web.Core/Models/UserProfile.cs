using Pulse.App.Dto.Users;

namespace Pulse.Web.Core.Models;

public sealed record UserProfile
{
    public required string Id { get; init; }
    
    public required string Username { get; init; }
}

public static class UserProfileExtensions
{
    public static UserProfile ToUserProfile(this AuthDto auth)
    {
        return new UserProfile
        {
            Id = auth.User.Id,
            Username = auth.User.Username
        };
    }
}