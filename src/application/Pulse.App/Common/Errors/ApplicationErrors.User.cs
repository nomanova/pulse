using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static class User
    {
        public static readonly Error InvalidCredentials =
            Error.Validation("User.InvalidCredentials", "Provided credentials are invalid");
    }
}