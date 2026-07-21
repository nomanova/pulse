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
        [CommandArgument(0, "[name]")]
        [Description("Name of the server")]
        public required string? Name { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var name = settings.Name;
        var config = _configService.Load();

        // Server name explicitly provided
        if (name is not null && config.Servers.ContainsKey(name))
        {
            SelectServer(config, name);
            return Exit.Success;
        }

        if (name is not null && !config.Servers.ContainsKey(name))
        {
            _console.WriteError($"No server '{name}' found");
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
                name = config.Servers.Keys.First();
                SelectServer(config, name);
                return Exit.Success;
        }

        // Selection menu
        var serverNames = config.Servers.Keys.ToArray();

        var selectedServer = _console.Prompt(new SelectionPrompt<string>()
            .Title("Select server")
            .PageSize(Constants.MaxServerCount)
            .AddChoices(serverNames)
        );

        SelectServer(config, selectedServer);

        return Exit.Success;
    }

    private void SelectServer(Config config, string name)
    {
        config.SetServer(name);
        _configService.Save(config);

        _console.WriteLine($"Selected server '{name}'");
    }
}