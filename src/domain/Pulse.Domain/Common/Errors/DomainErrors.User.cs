using ErrorOr;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class User
    {
        // Name
        public static readonly Error NameRequired = Required("User.NameRequired", "Name");
        
        // Email address
        public static readonly Error EmailAddressRequired = Required("User.EmailAddressRequired", "Email address");

        public static readonly Error EmailAddressInvalidFormat =
            Error.Validation("User.EmailAddressInvalidFormat", "Email address has an invalid format");

        public static readonly Error EmailAddressTooLong =
            TooLong("User.EmailAddressTooLong", "Email address", EmailAddress.MaxLength);
        
        public static readonly Error EmailAddressForbiddenCharacter =
            Error.Validation("User.EmailAddressForbiddenCharacter", "Email address contains forbidden characters");
        
        // Password
        public static readonly Error PasswordRequired = Required("User.PasswordRequired", "Password");

        public static readonly Error PasswordTooShort =
            TooShort("User.PasswordTooShort", "Password", Password.MinLength);

        public static readonly Error PasswordTooLong = TooLong("User.PasswordTooLong", "Password", Password.MaxLength);
    }
}