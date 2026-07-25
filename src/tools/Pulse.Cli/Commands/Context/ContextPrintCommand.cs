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

        table.AddRow("[grey]server[/]", config.Context.ServerName ?? NoValue);
        table.AddRow("[grey]organization[/]", config.Context.OrganizationName ?? NoValue);
        table.AddRow("[grey]application[/]", config.Context.ApplicationName ?? NoValue);
        table.AddRow("[grey]environment[/]", config.Context.EnvironmentName ?? NoValue);

        _console.WriteTable(table);

        return Exit.Success;
    }
}