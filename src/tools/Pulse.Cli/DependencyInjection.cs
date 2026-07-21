using System;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Services;
using Spectre.Console.Cli;

namespace Pulse.Cli;

public static class DependencyInjection
{
    private sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
    {
        public object? Resolve(Type? type) => type == null ? null : provider.GetService(type);
    }

    private sealed class TypeRegistrar(IServiceCollection services) : ITypeRegistrar
    {
        public ITypeResolver Build() => new TypeResolver(services.BuildServiceProvider());

        public void Register(Type service, Type implementation) => services.AddSingleton(service, implementation);

        public void RegisterInstance(Type service, object implementation) =>
            services.AddSingleton(service, implementation);

        public void RegisterLazy(Type service, Func<object> factory) => services.AddSingleton(service, _ => factory());
    }

    public static ITypeRegistrar GetRegistrar()
    {
        var services = new ServiceCollection();

        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<IEndpointProvider, EndpointProvider>();
        services.AddSingleton<ITokenProvider, TokenProvider>();

        services.AddSingleton<ICtrlApiClient>(provider => new CtrlApiClientBuilder()
            .WithEndpoint(provider.GetRequiredService<IEndpointProvider>())
            .WithToken(provider.GetRequiredService<ITokenProvider>())
            .Build());

        return new TypeRegistrar(services);
    }
}