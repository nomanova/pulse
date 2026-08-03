using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.App.Dto.Applications;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public sealed class AppListCommand : PagedAppCommand
{
    public const string CmdId = "list";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public AppListCommand(
        IAnsiConsole console,
        IConfigService configService,
        ICtrlApiClient ctrlApiClient) : base(console, configService, ctrlApiClient)
    {
        _console = console;
        _configService = configService;
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context, PagedCommandSettings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();
        config.AssertOrganization();

        var (lastId, entities) = await Fetch(settings, cancellationToken);

        if (entities is null)
        {
            return Exit.Error;
        }

        if (entities.Count == 0)
        {
            _console.WriteLine("No applications found");
            return Exit.Success;
        }

        Print(entities, config, lastId);

        return Exit.Success;
    }

    private void Print(List<ApplicationDto> entities, Config config, string? lastId)
    {
        var table = new Table();

        table.HideHeaders();
        table.Border(TableBorder.None);

        table.AddColumn("");
        table.AddColumn("Name");

        foreach (var entity in entities)
        {
            var isSelected = config.Context.Application?.Name == entity.Name;
            table.AddRow(isSelected ? "[grey]*[/]" : "", entity.Name);
        }

        _console.WriteTable(table);
        _console.WriteContinuation(lastId);
    }
}