using System.Collections.Generic;
using System.Linq;

namespace Pulse.Plugin;

public static class Extensions
{
    public static string? GetValue(this List<PluginParameterValue> values, string key)
    {
        return values.FirstOrDefault(value => value.Key == key)?.Value;
    }

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