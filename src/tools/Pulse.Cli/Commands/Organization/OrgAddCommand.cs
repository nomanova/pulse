using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public sealed class OrgAddCommand : AsyncCommand<OrgAddCommand.Settings>
{
    public const string CmdId = "add";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public OrgAddCommand(
        IAnsiConsole console,
        IConfigService configService,
        ICtrlApiClient ctrlApiClient)
    {
        _console = console;
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    public sealed class Settings : OrgSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Name of the organization")]
        public required string Name { get; init; }
        
        [CommandArgument(1, "<key>")]
        [Description("Key of the organization")]
        public required string Key { get; init; }
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        var config = _configService.Load();
        config.AssertServer();

        var name = settings.Name;
        var key = settings.Key;

        var request = new AddOrganizationRequest
        {
            OrganizationName = name,
            OrganizationKey = key
        };

        var result = await _ctrlApiClient.Organizations.Add(request, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        var id = result.Data.Id;
        
        config.SetOrganization(id, key); // Immediately select the new organization
        _configService.Save(config);

        _console.WriteLine($"Organization '{name}' ('{key}') added");
        _console.WriteLine($"Selected organization '{key}'");

        return Exit.Success;
    }
}