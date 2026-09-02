using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static class Connection
    {
        public static readonly Error NotProvider = 
            Error.Validation("Connection.NotProvider", "The requested plugin is not a provider");
        
        public static readonly Error Duplicate =
            Error.Validation("Connection.Duplicate", "A connection for the same plugin exists");
    }
}