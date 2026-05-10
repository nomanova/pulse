using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Throw;

namespace Pulse.Api.Common;

internal static class Endpoints
{
    internal static void AddEndpoints(this IServiceCollection services, params Assembly[] applicationParts)
    {
        services.ThrowIfNull();

        var mvcBuilder = services.AddControllers()
            .ConfigureApiBehaviorOptions(Set);

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
}