using System.ComponentModel;
using System.Threading.Tasks;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract.Users;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.User;

public sealed class UserSignInCommand : AsyncCommand<UserSignInCommand.Settings>
{
    public const string Name = "sign-in";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;
    private readonly ICtrlApiClient _ctrlApiClient;

    public UserSignInCommand(
        IAnsiConsole console, 
        IConfigService configService, 
        ICtrlApiClient ctrlApiClient)
    {
        _console = console;
        _configService = configService;
        _ctrlApiClient = ctrlApiClient;
    }

    public sealed class Settings : UserSettings
    {
        [CommandArgument(0, "[username]")]
        [Description("Username")]
        public string? Username { get; init; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var config = _configService.Load();

        if (!config.HasServer())
        {
            _console.WriteError("No server selected");
            return Constants.ExitError;
        }

        var username = settings.Username;

        if (username is null)
        {
            var usernamePrompt = new TextPrompt<string>("Username: ");
            username = _console.Prompt(usernamePrompt);
        }

        var passwordPrompt = new TextPrompt<string>("Password: ").Secret();
        var password = _console.Prompt(passwordPrompt);
        
        var request = new SignInRequest
        {
            Username = username,
            Password = password
        };

        var result = await _ctrlApiClient.Users.SignIn(request);

        if (!result.Success)
        {
            _console.WriteProblem(result.Problem!);
            return Constants.ExitError;
        }

        var accessToken = result.Data!.AccessToken;
        config.SignIn(accessToken);
        
        _configService.Save(config);

        _console.WriteLine($"User '{username}' signed in");
        
        return Constants.ExitSuccess;
    }
}