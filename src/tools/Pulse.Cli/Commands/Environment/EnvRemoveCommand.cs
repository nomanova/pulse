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

namespace Pulse.Cli.Commands.Environment;

public sealed class EnvRemoveCommand : AsyncCommand<EnvRemoveCommand.Settings>
{
    public const string CmdId = "remove";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public EnvRemoveCommand(
        IAnsiConsole console,
        IConfigService configService,
        ICtrlApiClient ctrlApiClient)
    {
        _console = console;
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    public sealed class Settings : EnvSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Name of the environment")]
        public required string Name { get; init; }
    }

    protected override async Task<int> ExecuteAsync(
        CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();
        config.AssertApplication();

        var name = settings.Name;
        var applicationId = config.Context.Application!.Id;

        // Search
        var searchRequest = new SearchEnvironmentsRequest
        {
            Query = name,
            ApplicationId = applicationId
        };

        var searchResult = await _ctrlApiClient.Environments.Search(searchRequest, cancellationToken);

        if (!searchResult.Success || searchResult.Data == null)
        {
            _console.WriteProblem(searchResult.Problem, searchResult.StatusCode);
            return Exit.Error;
        }

        var environment = searchResult.Data.Entities.FirstOrDefault();

        if (environment == null)
        {
            _console.WriteError($"Environment '{name}' not found");
            return Exit.Error;
        }

        // Remove
        var request = new RemoveEnvironmentRequest
        {
            EnvironmentId = environment.Id
        };

        var result = await _ctrlApiClient.Environments.Remove(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        if (config.Context.Environment?.Name == name)
        {
            config.ClearEnvironment();
        }

        _configService.Save(config);

        _console.WriteLine($"Environment '{name}' removed");

        return Exit.Success;
    }
}