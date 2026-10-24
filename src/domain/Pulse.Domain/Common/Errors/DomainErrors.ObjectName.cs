using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class ObjectName
    {
        public static readonly Error Required = Required("ObjectName.Required", "Name");
        
        public static readonly Error TooLong =
            TooLong("ObjectName.TooLong", "Name", Models.ValueObjects.ObjectName.MaxLength);

    }
}