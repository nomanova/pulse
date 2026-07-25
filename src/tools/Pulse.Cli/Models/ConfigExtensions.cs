namespace Pulse.Cli.Models;

public static class ConfigExtensions
{
    public static void AssertServer(this Config config)
    {
        if (!config.HasServer())
        {
            throw new CliException("No server selected");
        }
    }
}