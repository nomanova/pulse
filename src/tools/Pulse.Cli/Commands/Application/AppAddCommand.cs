using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public sealed class AppAddCommand : AsyncCommand<AppAddCommand.Settings>
{
    public const string CmdId = "add";
    public const string CmdAliasId = "create";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public AppAddCommand(
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

        var request = new CreateApplicationRequest
        {
            OrganizationName = config.Context.OrganizationName,
            ApplicationName = name
        };

        var result = await _ctrlApiClient.Applications.Create(request, cancellationToken);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        config.SetApplication(name); // Immediately select the new application
        _configService.Save(config);

        _console.WriteLine($"Application '{name}' added");
        _console.WriteLine($"Selected application '{name}'");

        return Exit.Success;
    }
}