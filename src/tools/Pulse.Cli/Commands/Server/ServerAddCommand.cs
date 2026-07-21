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
        [CommandArgument(0, "<name>")]
        [Description("Name of the server")]
        public required string Name { get; init; }

        [CommandArgument(1, "<url>")]
        [Description("URL of the server")]
        public required string Url { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var name = settings.Name;
        var config = _configService.Load();

        if (config.Servers.Count >= Constants.MaxServerCount)
        {
            _console.WriteError($"Maximum number of servers ({Constants.MaxServerCount}) reached");
            return Exit.Error;
        }

        if (config.Servers.ContainsKey(name))
        {
            _console.WriteError($"Server '{name}' exists");
            return Exit.Error;
        }

        config.Servers.Add(name, new Models.Server
        {
            Url = settings.Url
        });
        
        config.SetServer(name); // Immediately select the new server
        _configService.Save(config);

        _console.WriteLine($"Server '{name}' added");
        _console.WriteLine($"Selected server '{name}'");

        return Exit.Success;
    }
}