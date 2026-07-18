using Pulse.Cli.Commands.User;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.User;

public sealed class UserSignOutCommandTests : CliTests
{
    public UserSignOutCommandTests()
    {
        App.SetDefaultCommand<UserSignOutCommand>();
    }

    [Fact]
    public void Run_NoServerSelected_ShouldFail()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No server selected", result.Output);
    }

    [Fact]
    public void Run_ServerSelected_ShouldSignOut()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Null(ConfigService.SavedConfig.Servers["default"].AccessToken);
    }

    [Fact]
    public void Run_ServerSelected_ShouldSaveConfig()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.NotNull(ConfigService.SavedConfig);
    }

    [Fact]
    public void Run_ServerSelected_ShouldPrintSignedOut()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Signed out from 'default'", result.Output);
    }

    private static Config ServerConfig()
    {
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Models.Server
                {
                    Url = "http://localhost:5000",
                    AccessToken = "access-token"
                }
            }
        };

        config.SetServer("default");

        return config;
    }
}