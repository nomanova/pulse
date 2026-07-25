using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.App.Dto.Organizations;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public sealed class OrgListCommand : PagedOrgCommand
{
    public const string CmdId = "list";
    public const string CmdAliasId = "search";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public OrgListCommand(
        IAnsiConsole console,
        IConfigService configService,
        ICtrlApiClient ctrlApiClient) : base(console, ctrlApiClient)
    {
        _console = console;
        _configService = configService;
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context, PagedCommandSettings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();
        config.AssertServer();

        var (lastId, organizations) = await Fetch(settings, cancellationToken);

        if (organizations is null)
        {
            return Exit.Error;
        }

        if (organizations.Count == 0)
        {
            _console.WriteLine("No organizations found");
            return Exit.Success;
        }

        Print(organizations, config, lastId);

        return Exit.Success;
    }

    private void Print(List<OrganizationDto> organizations, Config config, string? lastId)
    {
        var table = new Table();

        table.HideHeaders();
        table.Border(TableBorder.None);

        table.AddColumn("");
        table.AddColumn("Name");

        foreach (var organization in organizations)
        {
            var isSelected = config.Context.OrganizationName == organization.Name;
            table.AddRow(isSelected ? "[grey]*[/]" : "", organization.Name);
        }

        _console.WriteTable(table);

        if (lastId is not null)
        {
            _console.WriteLine();
            _console.WriteLine($"Use `pulse org list -c {lastId}` to fetch more results");
        }
    }
}