using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Organization;

public sealed class OrgRemoveCommand : AsyncCommand<OrgRemoveCommand.Settings>
{
    public const string CmdId = "remove";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public OrgRemoveCommand(
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

        var request = new DeleteOrganizationRequest
        {
            OrganizationName = name
        };

        var result = await _ctrlApiClient.Organizations.Delete(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        if (config.Context.OrganizationName == name)
        {
            config.ClearOrganization();
        }

        _configService.Save(config);

        _console.WriteLine($"Organization '{name}' removed");

        return Exit.Success;
    }
}