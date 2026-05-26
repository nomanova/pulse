using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Behaviors;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Users.Common;

namespace Pulse.App;

public static class Setup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDispatcher(Assembly.GetExecutingAssembly());
        services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddRepositories();
        
        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        // (Write) repositories (aggregate roots)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
    }

}