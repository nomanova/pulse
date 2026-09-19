using System;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using MudExtensions.Services;
using Pulse.Api.Client.Common;
using Pulse.Api.Client.Handlers;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Data.Client;
using Pulse.Web.Common;
using Pulse.Web.Common.Navigation;
using Pulse.Web.Common.Security;
using Pulse.Web.Common.Services;
using Pulse.Web.Core;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web;

public static class Setup
{
    extension(WebAssemblyHostBuilder builder)
    {
        public WebAssemblyHostBuilder AddOptions()
        {
            builder.Services.AddOptions();
            return builder;
        }

        public WebAssemblyHostBuilder AddApi()
        {
            var apiEndpoint = builder.Configuration["Api:Endpoint"] ?? "https://localhost:5001/";
            var apiTimeout = int.Parse(builder.Configuration["Api:TimeoutInSeconds"] ?? "15");

            var environment = builder.HostEnvironment.Environment;

            Console.WriteLine($"Environment: {environment}");
            Console.WriteLine($"Api Endpoint: {apiEndpoint}");

            //builder.Services.AddScoped<IResponseHandler, GlobalResponseHandler>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            
            builder.Services.AddScoped<ICtrlApiClient>(provider =>
                new CtrlApiClientBuilder()
                    .WithEndpoint(new EndpointProvider(apiEndpoint))
                    .WithTimeout(TimeSpan.FromSeconds(apiTimeout))
                    //.WithResponseHandler(provider.GetRequiredService<IResponseHandler>())
                    .WithTokenProvider(provider.GetRequiredService<ITokenProvider>())
                    .Build()
            );

            builder.Services.AddScoped<IDataApiClient>(_ =>
                new DataApiClientBuilder()
                    .WithEndpoint(new EndpointProvider(apiEndpoint))
                    .WithTimeout(TimeSpan.FromSeconds(apiTimeout))
                    .Build()
            );

            return builder;
        }

        public WebAssemblyHostBuilder AddLogging()
        {
            builder.Logging.AddConfiguration(
                builder.Configuration.GetSection("Logging"));

            return builder;
        }

        public WebAssemblyHostBuilder AddMud()
        {
            builder.Services.AddMudServices(configuration =>
            {
                configuration.SnackbarConfiguration.MaximumOpacity = 100;
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;
                configuration.SnackbarConfiguration.NewestOnTop = false;
                configuration.SnackbarConfiguration.PreventDuplicates = false;
                configuration.SnackbarConfiguration.MaxDisplayedSnackbars = 5;
            });
            builder.Services.AddMudExtensions();

            builder.Services.AddScoped<IFlagManager, FlagManager>();

            return builder;
        }

        public WebAssemblyHostBuilder AddSecurity()
        {
            builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<UserAuthenticationStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
                 provider.GetRequiredService<UserAuthenticationStateProvider>());
            
            builder.Services.AddScoped<IAuthenticationStore, AuthenticationStore>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            
            return builder;
        }

        public WebAssemblyHostBuilder AddServices()
        {
            builder.Services.AddScoped<ILocalStorage, LocalStorage>();
            builder.Services.AddScoped<IPageNavigator, PageNavigator>();
            builder.Services.AddScoped<IClipboard, Clipboard>();

            return builder;
        }

        public WebAssemblyHostBuilder AddCore()
        {
            builder.Services.AddViewModels();
            builder.Services.AddCoreServices();

            return builder;
        }
    }
}