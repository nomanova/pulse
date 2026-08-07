using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Pulse.Api.Common;
using Pulse.Api.Data;
using Pulse.Infra.Database.Contexts;
using Scalar.AspNetCore;

namespace Pulse.Api;

internal static class Setup
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        var ctrlAssembly = typeof(Ctrl.AssemblyReference).Assembly;
        var dataAssembly = typeof(Data.AssemblyReference).Assembly;

        List<Assembly> applicationParts = [ctrlAssembly, dataAssembly];
        services.AddEndpoints(applicationParts.ToArray());

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info.Version = "1.0";
                document.Info.Title = "Pulse API";
                document.Info.Description = "Open-source .NET enterprise notification gateway.";
                document.Info.Contact = new OpenApiContact
                {
                    Name = "NOMANOVA",
                    Email = "info@nomanova.com",
                    Url = new Uri("https://nomanova.com")
                };
                document.Info.License = new OpenApiLicense
                {
                    Name = "AGPL v3.0",
                    Url = new Uri("https://www.gnu.org/licenses/agpl-3.0.html")
                };
                return Task.CompletedTask;
            });
        });

        services.AddAuthorizationBuilder()
            .SetFallbackPolicy(
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build())
            .AddPolicy(ApiKeyAuthenticationDefaults.Policy, policy =>
            {
                policy.AuthenticationSchemes.Add(ApiKeyAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            });
        
        return services;
    }

    public static void UsePresentation(this IApplicationBuilder app,
        IServiceProvider serviceProvider, DatabaseContext databaseContext)
    {
        databaseContext.EnsureMigrated();
        databaseContext.EnsureSeeded(serviceProvider);

        app.UseExceptionMiddleware();
        app.UseFileServer();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapOpenApi().AllowAnonymous();
            endpoints.MapScalarApiReference(options =>
            {
                options.DisableAgent();
                options.DisableMcp();
                options.HideClientButton();
                options.HideDeveloperTools();
                options.DisableTelemetry();
            }).AllowAnonymous();

            endpoints.MapControllers();
        });
    }
}