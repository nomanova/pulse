namespace Pulse.Cli.Models;

public sealed record Context
{
    public string? ServerName { get; init; }

    public NamedIdentity? Organization { get; init; }

    public NamedIdentity? Application { get; init; }

    public NamedIdentity? Environment { get; init; }
}