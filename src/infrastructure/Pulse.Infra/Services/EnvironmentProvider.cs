using System;
using Microsoft.Extensions.Hosting;
using Pulse.App.Common.Services.Interfaces;

namespace Pulse.Infra.Services;

public sealed class EnvironmentProvider : IEnvironmentProvider
{
    private const string AspNetCoreEnvironmentVar = "ASPNETCORE_ENVIRONMENT";
    
    // Hosted environment, serving real users
    private const string EnvProduction = "Production";
    
    // Virtual environment, used for integration testing (CI)
    private const string EnvIntegration = "Integration";
    
    // Virtual environment, representing the app running on the developer machine
    private const string EnvLocal = "Local";

    public EnvironmentProvider(IHostEnvironment hostEnvironment)
    {
        Name = hostEnvironment.EnvironmentName;
    }
    
    public string Name
    {
        get
        {
            if (!string.IsNullOrEmpty(field))
            {
                return field;
            }

            return Environment.GetEnvironmentVariable(AspNetCoreEnvironmentVar) ?? "Unknown";
        }
    } 
    
    public bool IsLocal => IsEnvironment(EnvLocal);
    
    public bool IsIntegration => IsEnvironment(EnvIntegration);
    
    public bool IsProduction => IsEnvironment(EnvProduction);

    public bool IsVirtual => IsLocal || IsIntegration;
    
    private bool IsEnvironment(string name)
    {
        return Name.Equals(name, StringComparison.InvariantCultureIgnoreCase);
    }
}