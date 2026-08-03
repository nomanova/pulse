using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Environment;

public sealed class EnvAddCommand : AsyncCommand<EnvAddCommand.Settings>
{
    public const string CmdId = "add";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public EnvAddCommand(
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

        var request = new AddEnvironmentRequest
        {
            ApplicationId = config.Context.Application!.Id,
            EnvironmentName = name
        };

        var result = await _ctrlApiClient.Environments.Add(request, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        var id = result.Data.Id;

        config.SetEnvironment(id, name); // Immediately select the new environment
        _configService.Save(config);

        _console.WriteLine($"Environment '{name}' added");
        _console.WriteLine($"Selected environment '{name}'");

        return Exit.Success;
    }
}