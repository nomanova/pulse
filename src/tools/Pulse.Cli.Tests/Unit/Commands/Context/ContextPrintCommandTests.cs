using Pulse.Cli.Commands.Context;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Context;

public sealed class ContextPrintCommandTests : CliTests
{
    public ContextPrintCommandTests()
    {
        App.SetDefaultCommand<ContextPrintCommand>();
    }
    
    [Fact]
    public void Run_ShouldSucceed()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
    }

    [Fact]
    public void Run_NoContextSelected_ShouldPrintNoValues()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("srv", result.Output);
        Assert.Contains("org", result.Output);
        Assert.Contains("app", result.Output);
        Assert.Contains("env", result.Output);
        Assert.Contains("<none>", result.Output);
    }

    [Fact]
    public void Run_ServerSelected_ShouldPrintServer()
    {
        // Arrange
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        };

        config.SetServer("default");

        ConfigService.UseConfig(config);

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("srv", result.Output);
        Assert.Contains("default", result.Output);
    }

    [Fact]
    public void Run_OrganizationSelected_ShouldPrintOrganization()
    {
        // Arrange
        var config = new Config
        {
            Servers =
            {
                ["default"] = new Models.Server
                {
                    Url = "http://localhost:5000"
                }
            }
        };

        config.SetServer("default");
        config.SetOrganization("org_1", "production");

        ConfigService.UseConfig(config);

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("org", result.Output);
        Assert.Contains("production", result.Output);
    }
}