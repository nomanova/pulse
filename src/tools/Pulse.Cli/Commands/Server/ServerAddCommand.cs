using System.ComponentModel;
using System.Threading;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerAddCommand : Command<ServerAddCommand.Settings>
{
    public const string CmdId = "add";
    
    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerAddCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings
    {
        [CommandArgument(0, "<key>")]
        [Description("Key of the server")]
        public required string Key { get; init; }

        [CommandArgument(1, "<url>")]
        [Description("URL of the server")]
        public required string Url { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var key = settings.Key;
        var config = _configService.Load();

        if (config.Servers.Count >= Constants.MaxServerCount)
        {
            _console.WriteError($"Maximum number of servers ({Constants.MaxServerCount}) reached");
            return Exit.Error;
        }

        if (config.Servers.ContainsKey(key))
        {
            _console.WriteError($"Server '{key}' exists");
            return Exit.Error;
        }

        config.Servers.Add(key, new Models.Server
        {
            Url = settings.Url
        });
        
        config.SetServer(key); // Immediately select the new server
        _configService.Save(config);

        _console.WriteLine($"Server '{key}' added");
        _console.WriteLine($"Selected server '{key}'");

        return Exit.Success;
    }
}