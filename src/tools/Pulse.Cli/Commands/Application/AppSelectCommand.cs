using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public sealed class AppSelectCommand : PagedAppCommand
{
    public const string CmdId = "select";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public AppSelectCommand(
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

        switch (entities.Count)
        {
            case 0:
                _console.WriteLine("No applications found");
                return Exit.Success;
            case 1:
                SelectEntity(config, entities[0].Id, entities[0].Key);
                return Exit.Success;
        }

        // Selection menu
        var keys = entities.Select(o => o.Key).ToArray();

        _console.WriteContinuation(lastId);

        var selectedEntityKey = await _console.PromptAsync(new SelectionPrompt<string>()
                .Title("Select application")
                .AddChoices(keys),
            cancellationToken: cancellationToken);

        var entity = entities.Single(o => o.Key == selectedEntityKey);
        
        SelectEntity(config, entity.Id, entity.Key);

        return Exit.Success;
    }

    private void SelectEntity(Config config, string id, string key)
    {
        config.SetApplication(id, key);
        _configService.Save(config);

        _console.WriteLine($"Selected application '{key}'");
    }
}