
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Web.Core;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddViewModels()
        {
            return services;
        }

        public IServiceCollection AddCoreServices()
        {
            return services;
        }
    }
}