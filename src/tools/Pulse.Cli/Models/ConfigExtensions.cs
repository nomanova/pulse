namespace Pulse.Cli.Models;

public static class ConfigExtensions
{
    extension(Config config)
    {
        public void AssertServer()
        {
            if (!config.HasServer())
            {
                throw new CliException("No server selected");
            }
        }

        public void AssertOrganization()
        {
            if (!config.HasOrganization())
            {
                throw new CliException("No organization selected");
            }
        }
        
        public void AssertApplication()
        {
            if (!config.HasApplication())
            {
                throw new CliException("No application selected");
            }
        }
    }
}