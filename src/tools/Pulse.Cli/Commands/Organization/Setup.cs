using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public static class Setup
{
    public static IConfigurator AddOrganization(this IConfigurator config)
    {
        config.AddBranch("org", configurator =>
        {
            configurator.SetDescription("Organization commands (org | organization)");
            
            configurator.AddCommand<OrgAddCommand>(OrgAddCommand.CmdId)
                .WithDescription("Add a new organization");
        }).WithAlias("organization");

        return config;
    }
}