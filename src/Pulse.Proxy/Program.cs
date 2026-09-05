using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Pulse.Proxy;

public static class Program
{
    private const int ExitSuccess = 0;
    
    public static int Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        
        var app = builder.Build();
        app.MapReverseProxy();
        app.Run();
        
        return ExitSuccess;
    }
}