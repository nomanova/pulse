using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static class Plugin
    {
        public static readonly Error ConnectionFailed = 
            Error.Validation("Plugin.ConnectionFailed", "Connection failed");
    }
}