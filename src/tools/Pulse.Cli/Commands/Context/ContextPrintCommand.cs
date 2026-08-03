using System.Threading;
using Pulse.Cli.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands.Context;

public sealed class ContextPrintCommand : Command<ContextPrintCommand.Settings>
{
    private const string NoValue = "<none>";

    private readonly IAnsiConsole _console;
    private readonly IConfigService _configService;

    public ContextPrintCommand(IAnsiConsole console, IConfigService configService)
    {
        _console = console;
        _configService = configService;
    }

    public sealed class Settings : ContextSettings;

    protected override int Execute(
        CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        var config = _configService.Load();

        var table = new Table();

        table.HideHeaders();
        table.Border(TableBorder.None);

        table.AddColumn("Key");
        table.AddColumn("Value");

        table.AddRow("[grey]srv[/]", config.Context.ServerName ?? NoValue);
        table.AddRow("[grey]org[/]", config.Context.Organization?.Name ?? NoValue);
        table.AddRow("[grey]app[/]", config.Context.Application?.Name ?? NoValue);
        table.AddRow("[grey]env[/]", config.Context.Environment?.Name ?? NoValue);

        _console.WriteTable(table);

        return Exit.Success;
    }
}