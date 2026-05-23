using ErrorOr;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.Domain.Common.Errors;

public static partial class BusinessErrors
{
    public static class User
    {
        // Email address
        public static readonly Error EmailAddressRequired = Required("User.EmailAddressRequired", "Email address");

        public static readonly Error EmailAddressInvalid =
            Error.Validation("User.EmailAddressInvalid", "Email address has an invalid format");

        public static readonly Error EmailAddressTooLong =
            TooLong("User.EmailAddressTooLong", "Email address", EmailAddress.MaxLength);

        // Password
        public static readonly Error PasswordRequired = Required("User.PasswordRequired", "Password");

        public static readonly Error PasswordTooShort =
            TooShort("User.PasswordTooShort", "Password", Password.MinLength);

        public static readonly Error PasswordTooLong = TooLong("User.PasswordTooLong", "Password", Password.MaxLength);
    }
}