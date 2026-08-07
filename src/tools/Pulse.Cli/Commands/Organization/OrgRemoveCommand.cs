using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Shared.Contract;
using Pulse.Cli.Models;
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
        config.AssertServer();

        var name = settings.Name;

        // Search
        var searchRequest = new NamedPagedSearchRequest
        {
            Query = name
        };
        
        var searchResult = await _ctrlApiClient.Organizations.Search(searchRequest, cancellationToken);
        
        if (!searchResult.Success || searchResult.Data == null)
        {
            _console.WriteProblem(searchResult.Problem, searchResult.StatusCode);
            return Exit.Error;
        }
        
        var organization = searchResult.Data.Entities.FirstOrDefault();

        if (organization == null)
        {
            _console.WriteError($"Organization '{name}' not found");
            return Exit.Error;
        }
        
        // Remove
        var request = new RemoveOrganizationRequest
        {
            OrganizationId = organization.Id
        };

        var result = await _ctrlApiClient.Organizations.Remove(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        if (config.Context.Organization?.Name == name)
        {
            config.ClearOrganization();
        }

        _configService.Save(config);

        _console.WriteLine($"Organization '{name}' removed");

        return Exit.Success;
    }
}