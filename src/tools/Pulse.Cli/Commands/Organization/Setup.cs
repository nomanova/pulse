using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public static class Setup
{
    public static IConfigurator AddOrganization(this IConfigurator config)
    {
        config.AddBranch("org", configurator =>
        {
            configurator.SetDescription("Organization commands");
            
            configurator.AddCommand<OrgAddCommand>(OrgAddCommand.CmdId)
                .WithDescription("Add a new organization");
            
            configurator.AddCommand<OrgRemoveCommand>(OrgRemoveCommand.CmdId)
                .WithDescription("Remove an organization");
            
            configurator.AddCommand<OrgListCommand>(OrgListCommand.CmdId)
                .WithDescription("List known organizations");
            
            configurator.AddCommand<OrgSelectCommand>(OrgSelectCommand.CmdId)
                .WithDescription("Select organization");
            
        }).WithAlias("organization");

        return config;
    }
}