using System.Collections.Generic;
using System.Linq;

namespace Pulse.Domain.Channels;

public static class Extensions
{
    public static string? GetValue(this List<ParameterValue> values, string key)
    {
        return values.FirstOrDefault(value => value.Key == key)?.Value;
    }
    
    public static List<ParameterValue> AsParameterValues(this Dictionary<string, string>? values)
    {
        if (values == null)
        {
            return [];
        }

        var parameters = new List<ParameterValue>();
        
        foreach (var (key, value) in values)
        {
            parameters.Add(new ParameterValue(key, value));
        }

        return parameters;
    }
}