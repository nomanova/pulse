using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Pulse.Api.Common;

internal static class JsonSettings
{
    public static JsonSerializerOptions Get()
    {
        var jsonOptions = new JsonOptions();
        var jsonSerializerOptions = jsonOptions.JsonSerializerOptions;
        
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        jsonSerializerOptions.WriteIndented = true;
        
        jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        
        return jsonSerializerOptions;
    }
}