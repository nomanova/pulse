namespace Pulse.Cli.Models;

public sealed record Context
{
    public string? ServerName { get; init; }

    public string? OrganizationName { get; init; }

    public string? ApplicationName { get; init; }

    public string? EnvironmentName { get; init; }
}