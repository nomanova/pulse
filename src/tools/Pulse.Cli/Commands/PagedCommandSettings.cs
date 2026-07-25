using System.ComponentModel;
using Spectre.Console.Cli;

namespace Pulse.Cli.Commands;

public class PagedCommandSettings : CommandSettings
{
    [CommandArgument(0, "[query]")]
    [Description("Filter by matching name")]
    public string? Query { get; init; }

    [CommandOption("-l|--limit")]
    [Description("Limit the max number of results per page (default: 20)")]
    public uint Limit { get; init; } = Constants.DefaultPageLimit;

    [CommandOption("-c|--cursor")]
    [Description("Fetch the next page of results")]
    public string? Cursor { get; init; }

    [CommandOption("-a|--all")]
    [Description("Fetch all results (default: false)")]
    public bool All { get; init; }
}

public static class PagedCommandExtensions
{
    public static void Assert(this PagedCommandSettings settings)
    {
        if (settings.Limit is < Constants.MinPageLimit or > Constants.MaxPageLimit)
        {
            throw new CliException($"Limit must be between {Constants.MinPageLimit} and {Constants.MaxPageLimit}");
        }
    }
}