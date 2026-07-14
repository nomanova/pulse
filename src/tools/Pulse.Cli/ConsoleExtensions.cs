using Spectre.Console;

namespace Pulse.Cli;

public static class ConsoleExtensions
{
    public static void WriteError(this IAnsiConsole console, string message)
    {
        console.Markup($"[red]{message}[/]");
        console.WriteLine();
    }
}