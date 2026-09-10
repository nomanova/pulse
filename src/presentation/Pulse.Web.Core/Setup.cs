
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Web.Core.ViewModels;

namespace Pulse.Web.Core;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddViewModels()
        {
            services.AddTransient<SignInViewModel>();
            
            return services;
        }

        public IServiceCollection AddCoreServices()
        {
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            
            return services;
        }
    }
}