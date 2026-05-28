using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class Membership
    {
        public static readonly Error InvalidRoleScope =
            Error.Validation("Membership.InvalidRoleScope", "Invalid role scope.");
    }
}