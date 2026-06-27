using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App;
using Pulse.Database.Contexts;
using Pulse.Infra;

namespace Pulse.Api;

internal class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddInfrastructure(_configuration)
            .AddApplication() 
            .AddPresentation();
    }

    public static void Configure(
        IApplicationBuilder app,
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        DatabaseContext databaseContext)
    {
        app.UsePresentation(serviceProvider, databaseContext);
    }
}