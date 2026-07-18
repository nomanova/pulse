using System.Threading;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.User;

public sealed class UserSignOutCommand : Command<UserSignOutCommand.Settings>
{
    public const string Name = "sign-out";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public UserSignOutCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : UserSettings;

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();

        if (!config.HasServer())
        {
            _console.WriteError("No server selected");
            return Exit.Error;
        }

        var serverName = config.Context.ServerName!;

        config.SignOut();
        _configService.Save(config);

        _console.WriteLine($"Signed out from '{serverName}'");

        return Exit.Success;
    }
}