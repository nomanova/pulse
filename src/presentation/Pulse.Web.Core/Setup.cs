
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Web.Core.ViewModels;
using ApplicationsViewModel = Pulse.Web.Core.ViewModels.Applications.ApplicationsViewModel;

namespace Pulse.Web.Core;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddViewModels()
        {
            services.AddTransient<SignInViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SelectOrganizationViewModel>();
            services.AddTransient<NewOrganizationViewModel>();
            services.AddTransient<ApplicationsViewModel>();
            
            return services;
        }

        public IServiceCollection AddCoreServices()
        {
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
            
            return services;
        }
    }
}