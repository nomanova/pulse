using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Context;

public static class Setup
{
    public static IConfigurator AddContext(this IConfigurator config)
    {
        config.AddCommand<ContextPrintCommand>("ctx")
            .WithDescription("Print the current context")
            .WithAlias("context");
        
        return config;
    }
}