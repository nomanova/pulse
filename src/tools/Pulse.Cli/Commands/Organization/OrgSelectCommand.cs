using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public sealed class OrgSelectCommand : PagedOrgCommand
{
    public const string CmdId = "select";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public OrgSelectCommand(
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

        if (organizations.Count == 1)
        {
            SelectOrganization(config, organizations[0].Name);
            return Exit.Success;
        }

        // Selection menu
        var orgNames = organizations.Select(o => o.Name).ToArray();

        if (lastId is not null)
        {
            _console.WriteLine();
            _console.WriteLine($"Use `pulse org select -c {lastId}` to fetch more results");
        }

        var selectedOrg = await _console.PromptAsync(new SelectionPrompt<string>()
                .Title("Select organization")
                .AddChoices(orgNames),
            cancellationToken: cancellationToken);

        SelectOrganization(config, selectedOrg);

        return Exit.Success;
    }

    private void SelectOrganization(Config config, string name)
    {
        config.SetOrganization(name);
        _configService.Save(config);

        _console.WriteLine($"Selected organization '{name}'");
    }
}