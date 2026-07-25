using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Environment;

public static class Setup
{
    public static IConfigurator AddEnvironment(this IConfigurator config)
    {
        config.AddBranch("env", configurator =>
        {
            configurator.SetDescription("Environment commands");
            
            configurator.AddCommand<EnvAddCommand>(EnvAddCommand.CmdId)
                .WithDescription("Add a new environment")
                .WithAlias(EnvAddCommand.CmdAliasId);
            
            configurator.AddCommand<EnvRemoveCommand>(EnvRemoveCommand.CmdId)
                .WithDescription("Remove an environment")
                .WithAlias(EnvRemoveCommand.CmdAliasId);
            
            configurator.AddCommand<EnvListCommand>(EnvListCommand.CmdId)
                .WithDescription("List known environments")
                .WithAlias(EnvListCommand.CmdAliasId);
            
            configurator.AddCommand<EnvSelectCommand>(EnvSelectCommand.CmdId)
                .WithDescription("Select environment");
            
        }).WithAlias("environment");

        return config;
    }
}