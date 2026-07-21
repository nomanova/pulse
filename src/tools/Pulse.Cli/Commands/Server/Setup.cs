using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public static class Setup
{
    public static IConfigurator AddServer(this IConfigurator config)
    {
        config.AddBranch("srv", configurator =>
        {
            configurator.SetDescription("Server commands (srv | server)");

            configurator.AddCommand<ServerAddCommand>(ServerAddCommand.CmdId)
                .WithDescription("Add a new server");

            configurator.AddCommand<ServerListCommand>(ServerListCommand.CmdId)
                .WithDescription("List known servers");

            configurator.AddCommand<ServerSelectCommand>(ServerSelectCommand.CmdId)
                .WithDescription("Select a server");

            configurator.AddCommand<ServerRemoveCommand>(ServerRemoveCommand.CmdId)
                .WithDescription("Remove a server");
        }).WithAlias("server");

        return config;
    }
}