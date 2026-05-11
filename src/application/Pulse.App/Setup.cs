using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Behaviors;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App;

public static class Setup
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddDispatcher(Assembly.GetExecutingAssembly());
        services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}