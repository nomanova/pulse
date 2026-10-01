using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static class Environment
    {
        public static readonly Error RemovePrimary = 
            Error.Validation("Environment.RemovePrimary", "Cannot remove primary environment");
    }
}