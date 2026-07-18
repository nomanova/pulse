using Pulse.Cli.Commands.Server;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Server;

public sealed class ServerListCommandTests : CliTests
{
    public ServerListCommandTests()
    {
        App.SetDefaultCommand<ServerListCommand>();
    }

    [Fact]
    public void Run_NoServers_ShouldSucceed()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
    }

    [Fact]
    public void Run_NoServers_ShouldPrintNoServersFound()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("No servers found", result.Output);
    }

    [Fact]
    public void Run_WithServers_ShouldSucceed()
    {
        // Arrange
        ConfigService.UseConfig(new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        });

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
    }

    [Fact]
    public void Run_WithServers_ShouldPrintServerNames()
    {
        // Arrange
        ConfigService.UseConfig(new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                },
                ["production"] = new Pulse.Cli.Models.Server
                {
                    Url = "https://pulse.example.com"
                }
            }
        });

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("default", result.Output);
        Assert.Contains("production", result.Output);
    }

    [Fact]
    public void Run_WithServers_ShouldPrintServerUrls()
    {
        // Arrange
        ConfigService.UseConfig(new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                },
                ["production"] = new Pulse.Cli.Models.Server
                {
                    Url = "https://pulse.example.com"
                }
            }
        });

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("http://localhost:5000", result.Output);
        Assert.Contains("https://pulse.example.com", result.Output);
    }

    [Fact]
    public void Run_WithSelectedServer_ShouldPrintSelectionMarker()
    {
        // Arrange
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                },
                ["production"] = new Pulse.Cli.Models.Server
                {
                    Url = "https://pulse.example.com"
                }
            }
        };

        config.SetServer("production");

        ConfigService.UseConfig(config);

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains(">", result.Output);
        Assert.Contains("production", result.Output);
    }

    [Fact]
    public void Run_WithServers_ShouldPrintTableHeaders()
    {
        // Arrange
        ConfigService.UseConfig(new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        });

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Name", result.Output);
        Assert.Contains("Url", result.Output);
    }
}