using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.User;

public static class Setup
{
    public static IConfigurator AddUser(this IConfigurator config)
    {
        config.AddBranch("user", configurator =>
        {
            configurator.SetDescription("User commands");
            
            configurator.AddCommand<UserSignInCommand>(UserSignInCommand.Name)
                .WithDescription("Sign in to current server");
            
            configurator.AddCommand<UserSignOutCommand>(UserSignOutCommand.Name)
                .WithDescription("Sign out from current server");
        });
        
        return config;
    }
}