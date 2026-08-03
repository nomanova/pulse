using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public sealed class AppRemoveCommand : AsyncCommand<AppRemoveCommand.Settings>
{
    public const string CmdId = "remove";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public AppRemoveCommand(
        IAnsiConsole console,
        IConfigService configService,
        ICtrlApiClient ctrlApiClient)
    {
        _console = console;
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    public sealed class Settings : AppSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Name of the application")]
        public required string Name { get; init; }
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();
        config.AssertOrganization();

        var name = settings.Name;

        // Search
        var searchRequest = new SearchApplicationsRequest
        {
            OrganizationId = config.Context.Organization!.Id,
            Query = name
        };
        
        var searchResult = await _ctrlApiClient.Applications.Search(searchRequest, cancellationToken);

        if (!searchResult.Success || searchResult.Data == null)
        {
            _console.WriteProblem(searchResult.Problem, searchResult.StatusCode);
            return Exit.Error;
        }
        
        var application = searchResult.Data.Entities.FirstOrDefault();

        if (application == null)
        {
            _console.WriteError($"Application '{name}' not found");
            return Exit.Error;
        }
        
        // Remove
        var request = new RemoveApplicationRequest
        {
            ApplicationId = application.Id
        };

        var result = await _ctrlApiClient.Applications.Remove(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        if (config.Context.Application?.Name == name)
        {
            config.ClearApplication();
        }

        _configService.Save(config);

        _console.WriteLine($"Application '{name}' removed");

        return Exit.Success;
    }
}