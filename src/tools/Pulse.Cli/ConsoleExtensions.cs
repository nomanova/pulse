using System.Linq;
using System.Net;
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

        public void WriteProblem(Problem? problem, HttpStatusCode status)
        {
            switch (problem)
            {
                case null:
                    console.Markup($"[red]{(int)status} - {status}[/]");
                    console.WriteLine();
                
                    return;
                case { Code: "General.Validation", ValidationErrors: not null }:
                {
                    var error = problem.ValidationErrors.FirstOrDefault();

                    if (error != null)
                    {
                        console.Markup($"[red]{error.Code} - {error.Description}[/]");
                        console.WriteLine();
                    }

                    return;
                }
                default:
                    console.Markup($"[red]{problem.Code} - {problem.Description}[/]");
                    console.WriteLine();
                    break;
            }
        }
    }
}