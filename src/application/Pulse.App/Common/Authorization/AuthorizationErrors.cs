using ErrorOr;

namespace Pulse.App.Common.Authorization;

public static class CustomErrorType
{
    public const int Forbidden = 20;
}

public static class CustomError
{
    public static Error Forbidden(
        string code = "General.Forbidden",
        string description = "Not enough permissions") => Error.Custom(
        type: CustomErrorType.Forbidden,
        code: code,
        description: description);
}

public static class AuthorizationErrors
{
    public static readonly Error UserSecurityStamp =
        CustomError.Forbidden("Authorization.UserSecurityStamp", "User security stamp has changed");
    
    public static readonly Error InsufficientPermissions =
        CustomError.Forbidden("Authorization.InsufficientPermissions", "User has insufficient permissions");
    
    public static readonly Error InvalidApiKey =
        CustomError.Forbidden("Authorization.InvalidApiKey", "Invalid api key");
}