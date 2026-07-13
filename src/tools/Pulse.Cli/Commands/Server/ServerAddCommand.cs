using System.ComponentModel;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerAddCommand : Command<ServerAddCommand.Settings>
{
    public const string Name = "add";

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

    public override int Execute(CommandContext context, Settings settings)
    {
        var name = settings.Name;
        var config = _configService.Load();

        if (config.Servers.ContainsKey(name))
        {
            _console.WriteError($"Server with name '{name}' exists");
            return Constants.ExitError;
        }

        config.Servers.Add(name, new Models.Server
        {
            Url = settings.Url
        });
        _configService.Save(config);

        _console.WriteLine($"Server '{name}' added");

        return Constants.ExitSuccess;
    }
}