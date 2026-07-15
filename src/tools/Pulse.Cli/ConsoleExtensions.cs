using System.Linq;
using Pulse.Api.Shared.Contract;
using Spectre.Console;

namespace Pulse.Cli;

public static class ConsoleExtensions
{
    extension(IAnsiConsole console)
    {
        public void WriteError(string message)
        {
            console.Markup($"[red]{message}[/]");
            console.WriteLine();
        }

        public void WriteProblem(Problem problem)
        {
            if (problem is { Code: "General.Validation", ValidationErrors: not null })
            {
                var error = problem.ValidationErrors.FirstOrDefault();

                if (error != null)
                {
                    console.Markup($"[red]{error.Code} - {error.Description}[/]");
                    console.WriteLine();
                }

                return;
            }

            console.Markup($"[red]{problem.Code} - {problem.Description}[/]");
            console.WriteLine();
        }
    }
}