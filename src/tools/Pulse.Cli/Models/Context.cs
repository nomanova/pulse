namespace Pulse.Cli.Models;

public sealed record Context
{
    public string? ServerKey { get; init; }

    public KeyedIdentity? Organization { get; init; }

    public KeyedIdentity? Application { get; init; }

    public KeyedIdentity? Environment { get; init; }
}