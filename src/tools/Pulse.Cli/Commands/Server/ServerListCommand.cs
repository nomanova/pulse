using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerListCommand : Command<ServerListCommand.Settings>
{
    public const string Name = "list";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerListCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings;

    public override int Execute(CommandContext context, Settings settings)
    {
        var config = _configService.Load();

        if (config.Servers.Count == 0)
        {
            _console.WriteLine("No servers found");
            return Constants.ExitSuccess;
        }

        foreach (var server in config.Servers)
        {
            _console.WriteLine($"{server.Key} - {server.Value.Url}");
        }

        return Constants.ExitSuccess;
    }
}