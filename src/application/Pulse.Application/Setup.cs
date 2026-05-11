using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Application.Common.Behaviors;
using Pulse.Application.Common.Dispatcher;

namespace Pulse.Application;

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