using Pulse.Cli.Commands.Server;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Server;

public sealed class ServerAddCommandTests : CliTests
{
    public ServerAddCommandTests()
    {
        App.SetDefaultCommand<ServerAddCommand>();
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
    public void Run_NameOnly_ShouldRequireUrl()
    {
        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("url", result.Output);
    }

    [Fact]
    public void Run_NameAndUrl_ShouldSucceed()
    {
        // Act
        var result = App.Run("default", "http://localhost:5000");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.True(ConfigService.SavedConfig.Servers.ContainsKey("default"));
    }

    [Fact]
    public void Run_NameAndUrl_ShouldSaveServerUrl()
    {
        // Act
        var result = App.Run("default", "http://localhost:5000");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("http://localhost:5000", ConfigService.SavedConfig.Servers["default"].Url);
    }

    [Fact]
    public void Run_NameAndUrl_ShouldSelectAddedServer()
    {
        // Act
        var result = App.Run("default", "http://localhost:5000");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("default", ConfigService.SavedConfig.Context.ServerName);
    }
    
    [Fact]
    public void Run_NameAndUrl_ShouldPrintServerAdded()
    {
        // Act
        var result = App.Run("default", "http://localhost:5000");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Server 'default' added", result.Output);
    }

    [Fact]
    public void Run_ServerExists_ShouldFail()
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
        var result = App.Run("default", "http://localhost:6000");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("Server 'default' exists", result.Output);
        Assert.Equal("http://localhost:5000", ConfigService.Config.Servers["default"].Url);
    }

    [Fact]
    public void Run_MaximumServerCountReached_ShouldFail()
    {
        // Arrange
        var config = new Config();

        for (var i = 0; i < Constants.MaxServerCount; i++)
        {
            config.Servers[$"server-{i}"] = new Pulse.Cli.Models.Server
            {
                Url = $"http://localhost:{5000 + i}"
            };
        }

        ConfigService.UseConfig(config);

        // Act
        var result = App.Run("extra", "http://localhost:6000");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains($"Maximum number of servers ({Constants.MaxServerCount}) reached", result.Output);
        Assert.False(ConfigService.SavedConfig.Servers.ContainsKey("extra"));
    }
}