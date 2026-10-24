using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class ObjectKey
    {
        public static readonly Error Required = Required("ObjectKey.Required", "Key");

        public static readonly Error InvalidFormat =
            Error.Validation("ObjectKey.InvalidFormat", "Key has an invalid format");

        public static readonly Error TooLong =
            TooLong("ObjectKey.TooLong", "Key", Models.ValueObjects.ObjectKey.MaxLength);

        public static readonly Error ForbiddenCharacter =
            Error.Validation("ObjectKey.ForbiddenCharacter", "Key contains forbidden characters");
    }
}