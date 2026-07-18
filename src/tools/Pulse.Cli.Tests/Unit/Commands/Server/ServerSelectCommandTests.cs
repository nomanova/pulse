using Pulse.Cli.Commands.Server;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Server;

public sealed class ServerSelectCommandTests : CliTests
{
    public ServerSelectCommandTests()
    {
        App.SetDefaultCommand<ServerSelectCommand>();
    }

    [Fact]
    public void Run_NoServers_ShouldFail()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
    }

    [Fact]
    public void Run_NoServers_ShouldPrintNoServersAvailable()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No servers available", result.Output);
    }

    [Fact]
    public void Run_ServerDoesNotExist_ShouldFail()
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
    }

    [Fact]
    public void Run_ServerDoesNotExist_ShouldPrintNoServerFound()
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
        Assert.Contains("No server 'default' found", result.Output);
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
    public void Run_ServerExists_ShouldSaveSelectedServer()
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
        Assert.Equal("default", ConfigService.SavedConfig.Context.ServerName);
    }

    [Fact]
    public void Run_ServerExists_ShouldPrintSelectedServer()
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
        Assert.Contains("Selected server 'default'", result.Output);
    }

    [Fact]
    public void Run_SingleServerWithoutName_ShouldSucceed()
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
    public void Run_SingleServerWithoutName_ShouldSaveSelectedServer()
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
        Assert.Equal("default", ConfigService.SavedConfig.Context.ServerName);
    }

    [Fact]
    public void Run_SingleServerWithoutName_ShouldPrintSelectedServer()
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
        Assert.Contains("Selected server 'default'", result.Output);
    }

    [Fact]
    public void Run_ServerExists_ShouldKeepServers()
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
        var result = App.Run("production");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.True(ConfigService.SavedConfig.Servers.ContainsKey("default"));
        Assert.True(ConfigService.SavedConfig.Servers.ContainsKey("production"));
    }
}