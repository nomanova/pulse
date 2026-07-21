using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.User;

public static class Setup
{
    public static IConfigurator AddUser(this IConfigurator config)
    {
        config.AddBranch("usr", configurator =>
        {
            configurator.SetDescription("User commands (usr | user)");
            
            configurator.AddCommand<UserSignInCommand>(UserSignInCommand.CmdId)
                .WithDescription("Sign in to current server");
            
            configurator.AddCommand<UserSignOutCommand>(UserSignOutCommand.CmdId)
                .WithDescription("Sign out from current server");
        }).WithAlias("user");
        
        return config;
    }
}