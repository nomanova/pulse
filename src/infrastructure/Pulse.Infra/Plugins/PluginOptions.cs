namespace Pulse.Infra.Plugins;

public sealed record PluginOptions
{
    public const string Section = "Plugins";
    
    public string? RootPath { get; init; }
}