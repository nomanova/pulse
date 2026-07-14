using System.ComponentModel;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerRemoveCommand : Command<ServerRemoveCommand.Settings>
{
    public const string Name = "remove";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerRemoveCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("Name of the server")]
        public required string Name { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name;
        var config = _configService.Load();

        if (!config.Servers.Remove(name))
        {
            _console.WriteError($"No server '{name}' found");
            return Constants.ExitError;
        }

        if (config.Context.ServerName == name)
        {
            config.ClearServer();
        }

        _configService.Save(config);

        _console.WriteLine($"Server '{name}' removed");

        return Constants.ExitSuccess;
    }
}