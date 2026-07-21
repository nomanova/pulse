using System.Threading.Tasks;
using Pulse.Cli.Commands.Organization;
using Pulse.Cli.Commands.Server;
using Pulse.Cli.Commands.User;
using Spectre.Console.Cli;

namespace Pulse.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var registrar = DependencyInjection.GetRegistrar();
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("pulse");

            config.AddServer();
            config.AddUser();
            config.AddOrganization();
        });

        return await app.RunAsync(args);
    }
}