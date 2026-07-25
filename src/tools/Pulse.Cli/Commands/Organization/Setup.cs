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
                .WithDescription("Add a new organization")
                .WithAlias(OrgAddCommand.CmdAliasId);
            
            configurator.AddCommand<OrgRemoveCommand>(OrgRemoveCommand.CmdId)
                .WithDescription("Remove an organization")
                .WithAlias(OrgRemoveCommand.CmdAliasId);
            
            configurator.AddCommand<OrgListCommand>(OrgListCommand.CmdId)
                .WithDescription("List known organizations")
                .WithAlias(OrgListCommand.CmdAliasId);
            
            configurator.AddCommand<OrgSelectCommand>(OrgSelectCommand.CmdId)
                .WithDescription("Select organization");
            
        }).WithAlias("organization");

        return config;
    }
}