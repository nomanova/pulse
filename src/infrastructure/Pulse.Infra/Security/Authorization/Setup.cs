using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Security.Interfaces;

namespace Pulse.Infra.Security.Authorization;

public static class Setup
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddScoped<IUserClaimProvider, UserClaimProvider>();

        return services;
    }
}