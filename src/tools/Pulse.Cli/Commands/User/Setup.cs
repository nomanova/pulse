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
                .WithDescription("Sign in");
        });
        
        return config;
    }
}