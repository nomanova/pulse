using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class ObjectName
    {
        public static readonly Error Required = Required("ObjectName.Required", "Name");

        public static readonly Error InvalidFormat =
            Error.Validation("ObjectName.InvalidFormat", "Name has an invalid format");

        public static readonly Error TooLong =
            TooLong("ObjectName.TooLong", "Name", Models.ValueObjects.ObjectName.MaxLength);

        public static readonly Error ForbiddenCharacter =
            Error.Validation("ObjectName.ForbiddenCharacter", "Name contains forbidden characters");
    }
}