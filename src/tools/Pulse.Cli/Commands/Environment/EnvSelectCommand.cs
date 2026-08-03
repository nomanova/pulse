using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Environment;

public sealed class EnvSelectCommand : PagedEnvCommand
{
    public const string CmdId = "select";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public EnvSelectCommand(
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
        config.AssertApplication();

        var (lastId, entities) = await Fetch(settings, cancellationToken);

        if (entities is null)
        {
            return Exit.Error;
        }

        switch (entities.Count)
        {
            case 0:
                _console.WriteLine("No environments found");
                return Exit.Success;
            case 1:
                SelectEntity(config, entities[0].Id, entities[0].Name);
                return Exit.Success;
        }

        // Selection menu
        var names = entities.Select(o => o.Name).ToArray();

        _console.WriteContinuation(lastId);

        var selectedEntityName = await _console.PromptAsync(new SelectionPrompt<string>()
                .Title("Select environment")
                .AddChoices(names),
            cancellationToken: cancellationToken);

        var entity = entities.Single(o => o.Name == selectedEntityName);

        SelectEntity(config, entity.Id, entity.Name);

        return Exit.Success;
    }

    private void SelectEntity(Config config, string id, string name)
    {
        config.SetEnvironment(id, name);
        _configService.Save(config);

        _console.WriteLine($"Selected environment '{name}'");
    }
}