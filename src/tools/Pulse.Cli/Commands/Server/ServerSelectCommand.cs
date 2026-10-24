using System.ComponentModel;
using System.Linq;
using System.Threading;
using Pulse.Cli.Models;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerSelectCommand : Command<ServerSelectCommand.Settings>
{
    public const string CmdId = "select";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerSelectCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings
    {
        [CommandArgument(0, "[key]")]
        [Description("Key of the server")]
        public required string? Key { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var key = settings.Key;
        var config = _configService.Load();

        // Server key explicitly provided
        if (key is not null && config.Servers.ContainsKey(key))
        {
            SelectServer(config, key);
            return Exit.Success;
        }

        if (key is not null && !config.Servers.ContainsKey(key))
        {
            _console.WriteError($"No server '{key}' found");
            return Exit.Error;
        }

        switch (config.Servers.Count)
        {
            // No servers available
            case 0:
                _console.WriteError("No servers available");
                return Exit.Error;
            // Single server available
            case 1:
                key = config.Servers.Keys.First();
                SelectServer(config, key);
                return Exit.Success;
        }

        // Selection menu
        var serverKeys = config.Servers.Keys.ToArray();

        var selectedServer = _console.Prompt(new SelectionPrompt<string>()
            .Title("Select server")
            .AddChoices(serverKeys)
        );

        SelectServer(config, selectedServer);

        return Exit.Success;
    }

    private void SelectServer(Config config, string key)
    {
        config.SetServer(key);
        _configService.Save(config);

        _console.WriteLine($"Selected server '{key}'");
    }
}