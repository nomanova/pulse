using System.Collections.Generic;
using System.Linq;

namespace Pulse.Plugin;

public static class Extensions
{
    public static string? GetValue(this List<PluginParameterValue> values, string key)
    {
        return values.FirstOrDefault(value => value.Key == key)?.Value;
    }

    public static List<PluginParameterValue> AsParameterValues(this Dictionary<string, string>? values)
    {
        if (values == null)
        {
            return [];
        }

        var parameters = new List<PluginParameterValue>();
        
        foreach (var (key, value) in values)
        {
            parameters.Add(new PluginParameterValue(key, value));
        }

        return parameters;
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