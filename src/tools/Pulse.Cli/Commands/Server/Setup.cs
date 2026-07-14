using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public static class Setup
{
    public static IConfigurator AddServer(this IConfigurator config)
    {
        config.AddBranch("server", configurator =>
        {
            configurator.SetDescription("Server commands");

            configurator.AddCommand<ServerAddCommand>(ServerAddCommand.Name)
                .WithDescription("Add a new server");

            configurator.AddCommand<ServerListCommand>(ServerListCommand.Name)
                .WithDescription("List known servers");

            configurator.AddCommand<ServerSelectCommand>(ServerSelectCommand.Name)
                .WithDescription("Select a server");

            configurator.AddCommand<ServerRemoveCommand>(ServerRemoveCommand.Name)
                .WithDescription("Remove a server");
        });

        return config;
    }
}