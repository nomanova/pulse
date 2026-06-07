using ErrorOr;
using Pulse.Domain.Common.Errors;

namespace Pulse.App.Common.Authorization;

public static class AuthorizationErrors
{
    public static readonly Error UserSecurityStamp =
        CustomError.Forbidden("Authorization.UserSecurityStamp", "User security stamp has changed");
    
    public static readonly Error InsufficientPermissions =
        CustomError.Forbidden("Authorization.InsufficientPermissions", "User has insufficient permissions");
}