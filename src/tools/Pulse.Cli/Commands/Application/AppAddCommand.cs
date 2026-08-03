using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Application;

public sealed class AppAddCommand : AsyncCommand<AppAddCommand.Settings>
{
    public const string CmdId = "add";

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

        var request = new AddApplicationRequest
        {
            OrganizationId = config.Context.Organization!.Id,
            ApplicationName = name
        };

        var result = await _ctrlApiClient.Applications.Add(request, cancellationToken);

        if (!result.Success || result.Data == null)
        {
            _console.WriteProblem(result.Problem, result.StatusCode);
            return Exit.Error;
        }

        var id = result.Data.Id;
        
        config.SetApplication(id, name); // Immediately select the new application
        _configService.Save(config);

        _console.WriteLine($"Application '{name}' added");
        _console.WriteLine($"Selected application '{name}'");

        return Exit.Success;
    }
}