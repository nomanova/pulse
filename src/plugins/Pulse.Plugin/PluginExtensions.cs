namespace Pulse.Plugin;

public static class Extensions
{
    extension(IPluginHostContext hostContext)
    {
        public void LogInformation(string message)
        {
            hostContext.Log(PluginLogLevel.Information, message);
        }

        public void LogError(string message)
        {
            hostContext.Log(PluginLogLevel.Error, message);
        }
    }
}