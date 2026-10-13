using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Web.Core.Services;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels;
using Pulse.Web.Core.ViewModels.Applications;
using Pulse.Web.Core.ViewModels.Applications.Environments;

namespace Pulse.Web.Core;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddViewModels()
        {
            services.AddTransient<UserViewModel>();
            services.AddTransient<SignInViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SelectOrganizationViewModel>();
            services.AddTransient<NewOrganizationViewModel>();

            services.AddApplicationViewModels();
            services.AddEnvironmentViewModels();
            
            return services;
        }

        public IServiceCollection AddCoreServices()
        {
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            services.AddSingleton<IContextProvider, ContextProvider>();
            
            return services;
        }
    }
}