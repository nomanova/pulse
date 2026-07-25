using System.Threading.Tasks;
using Pulse.Cli.Commands.Context;
using Pulse.Cli.Commands.Organization;
using Pulse.Cli.Commands.Server;
using Pulse.Cli.Commands.User;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var console = AnsiConsole.Console;
        var registrar = DependencyInjection.GetRegistrar(console);
        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("pulse");

            config.AddContext();
            config.AddServer();
            config.AddUser();
            config.AddOrganization();
        });

        try
        {
            return await app.RunAsync(args);
        }
        catch (CliException ex)
        {
            console.WriteError(ex.Message);
            return Exit.Error;
        }
    }
}