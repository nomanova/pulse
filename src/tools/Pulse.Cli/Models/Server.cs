namespace Pulse.Cli.Models;

public sealed record Server
{
    public required string Url { get; init; }

    public string? AccessToken { get; init; }
}