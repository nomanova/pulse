using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.App.Common.Authorization;

public static class AuthorizationRegistration
{
    extension(IServiceCollection services)
    {
        public void AddAuthorizersFromAssembly(Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            var authorizerType = typeof(IAuthorizer<>);
        
            assembly.GetTypesAssignableTo(authorizerType).ForEach(type =>
            {
                foreach (var implementedInterface in type.ImplementedInterfaces)
                {
                    switch (lifetime)
                    {
                        case ServiceLifetime.Scoped:
                            services.AddScoped(implementedInterface, type);
                            break;
                        case ServiceLifetime.Singleton:
                            services.AddSingleton(implementedInterface, type);
                            break;
                        case ServiceLifetime.Transient:
                            services.AddTransient(implementedInterface, type);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            });
        }

        public IServiceCollection AddAuthorizationRequirementsFromAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType)
                    {
                        continue;
                    }

                    if (iface.GetGenericTypeDefinition() == typeof(IAuthorizationHandler<>))
                    {
                        services.AddScoped(iface, type);
                    }
                }
            }

            services.AddScoped<IAuthorizationSender, AuthorizationSender>();

            return services;
        }
    }

    private static List<TypeInfo> GetTypesAssignableTo(this Assembly assembly, Type compareType)
    {
        return assembly.DefinedTypes.Where(typeInfo => typeInfo is { IsClass: true, IsAbstract: false }
                                                       && typeInfo != compareType
                                                       && typeInfo.GetInterfaces()
                                                           .Any(i => i.IsGenericType
                                                                     && i.GetGenericTypeDefinition() ==
                                                                     compareType)).ToList();
    }
}