using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Throw;

namespace Pulse.Api.Common;

internal static class Endpoints
{
    internal static void AddEndpoints(this IServiceCollection services, params Assembly[] applicationParts)
    {
        services.ThrowIfNull();

        var mvcBuilder = services.AddControllers()
            .ConfigureApiBehaviorOptions(Set)
            .AddJsonOptions(Set);

        foreach (var applicationPart in applicationParts)
        {
            mvcBuilder.AddApplicationPart(applicationPart);
        }
    }

    private static void Set(ApiBehaviorOptions options)
    {
        options.SuppressModelStateInvalidFilter = true;
        options.SuppressMapClientErrors = true;
    }
    
    private static void Set(JsonOptions options)
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
        
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    }
}