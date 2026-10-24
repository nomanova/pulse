using System.ComponentModel;
using System.Threading;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerRemoveCommand : Command<ServerRemoveCommand.Settings>
{
    public const string CmdId = "remove";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerRemoveCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings
    {
        [CommandArgument(0, "<key>")]
        [Description("Key of the server")]
        public required string Key { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var key = settings.Key;
        var config = _configService.Load();

        if (!config.Servers.Remove(key))
        {
            _console.WriteError($"No server '{key}' found");
            return Exit.Error;
        }

        if (config.Context.ServerKey == key)
        {
            config.ClearServer();
        }

        _configService.Save(config);

        _console.WriteLine($"Server '{key}' removed");

        return Exit.Success;
    }
}