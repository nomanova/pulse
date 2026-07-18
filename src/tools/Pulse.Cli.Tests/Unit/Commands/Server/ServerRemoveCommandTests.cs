using Pulse.Cli.Commands.Server;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Server;

public sealed class ServerRemoveCommandTests : CliTests
{
    public ServerRemoveCommandTests()
    {
        App.SetDefaultCommand<ServerRemoveCommand>();
    }
    
    [Fact]
    public void Run_NoArguments_ShouldPrintInstructions()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Instructions, result.ExitCode);
        Assert.Contains("ARGUMENTS", result.Output);
    }

    [Fact]
    public void Run_ServerExists_ShouldSucceed()
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
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
    }

    [Fact]
    public void Run_ServerExists_ShouldRemoveServer()
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
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.False(ConfigService.SavedConfig.Servers.ContainsKey("default"));
    }

    [Fact]
    public void Run_ServerExists_ShouldPrintServerRemoved()
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
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Server 'default' removed", result.Output);
    }

    [Fact]
    public void Run_ServerDoesNotExist_ShouldFail()
    {
        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No server 'default' found", result.Output);
    }

    [Fact]
    public void Run_ServerDoesNotExist_ShouldNotRemoveOtherServers()
    {
        // Arrange
        ConfigService.UseConfig(new Config
        {
            Servers =
            {
                ["production"] = new Pulse.Cli.Models.Server
                {
                    Url = "https://pulse.example.com"
                }
            }
        });

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.True(ConfigService.Config.Servers.ContainsKey("production"));
    }

    [Fact]
    public void Run_SelectedServer_ShouldClearSelectedServer()
    {
        // Arrange
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Pulse.Cli.Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        };

        config.SetServer("default");

        ConfigService.UseConfig(config);

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.False(ConfigService.SavedConfig.HasServer());
    }

    [Fact]
    public void Run_UnselectedServer_ShouldKeepSelectedServer()
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
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.True(ConfigService.SavedConfig.HasServer());
        Assert.Equal("production", ConfigService.SavedConfig.Context.ServerName);
    }
}