using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract.Organizations;
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
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings,
        CancellationToken cancellationToken)
    {
        var config = _configService.Load();

        if (!config.HasServer())
        {
            _console.WriteError("No server selected");
            return Exit.Error;
        }

        var name = settings.Name;

        var request = new CreateOrganizationRequest
        {
            OrganizationName = name
        };

        var result = await _ctrlApiClient.Organizations.Create(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        config.SetOrganization(name); // Immediately select the new organization
        _configService.Save(config);

        _console.WriteLine($"Organization '{name}' added");
        _console.WriteLine($"Selected organization '{name}'");

        return Exit.Success;
    }
}