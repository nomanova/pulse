using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public static class Setup
{
    public static IConfigurator AddApplication(this IConfigurator config)
    {
        config.AddBranch("app", configurator =>
        {
            configurator.SetDescription("Application commands");

            configurator.AddCommand<AppAddCommand>(AppAddCommand.CmdId)
                .WithDescription("Add a new application");

            configurator.AddCommand<AppRemoveCommand>(AppRemoveCommand.CmdId)
                .WithDescription("Remove an application");

            configurator.AddCommand<AppListCommand>(AppListCommand.CmdId)
                .WithDescription("List known applications");

            configurator.AddCommand<AppSelectCommand>(AppSelectCommand.CmdId)
                .WithDescription("Select application");
            
        }).WithAlias("application");

        return config;
    }
}