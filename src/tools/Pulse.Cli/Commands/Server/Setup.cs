using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public static class Setup
{
    public static IConfigurator AddServer(this IConfigurator config)
    {
        config.AddBranch("server", server =>
        {
            server.SetDescription("Server commands");

            server.AddCommand<ServerAddCommand>(ServerAddCommand.Name)
                .WithDescription("Add a new server");
            
            server.AddCommand<ServerListCommand>(ServerListCommand.Name)
                .WithDescription("List known servers");
        });
        
        return config;
    }
}