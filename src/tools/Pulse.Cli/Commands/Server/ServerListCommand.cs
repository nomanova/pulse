using System.Threading;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Server;

public sealed class ServerListCommand : Command<ServerListCommand.Settings>
{
    public const string CmdId = "list";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ServerListCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ServerSettings;

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();

        if (config.Servers.Count == 0)
        {
            _console.WriteLine("No servers found");
            return Exit.Success;
        }

        var table = new Table();

        table.AddColumn("");
        table.AddColumn("Name");
        table.AddColumn("Url");

        foreach (var server in config.Servers)
        {
            var isSelected = config.Context.ServerName == server.Key;
            table.AddRow(isSelected ? ">" : "", server.Key, server.Value.Url);
        }

        _console.Write(table);

        return Exit.Success;
    }
}