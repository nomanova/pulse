using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.Cli.Commands.Environment;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Environment;

public sealed class EnvRemoveCommandTests : CliTests
{
    public EnvRemoveCommandTests()
    {
        App.SetDefaultCommand<EnvRemoveCommand>();
    }

    [Fact]
    public void Run_NoArguments_ShouldFail()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.NotEqual(Exit.Success, result.ExitCode);
    }

    [Fact]
    public void Run_NoApplicationSelected_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        ConfigService.UseConfig(config);

        // Act
        var result = App.Run("prod");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No application selected", result.Output);
    }

    [Fact]
    public void Run_Name_ShouldRemoveEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Delete(It.IsAny<DeleteEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("prod");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        CtrlApiClient.EnvironmentsMock.Verify(x => x.Delete(
            It.Is<DeleteEnvironmentRequest>(r => r.EnvironmentName == "prod" && r.OrganizationName == "myorg" && r.ApplicationName == "myapp"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Run_SelectedEnvironment_ShouldClearSelectedEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        config.SetEnvironment("prod");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Delete(It.IsAny<DeleteEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        App.Run("prod");

        // Assert
        var savedConfig = ConfigService.Load();
        Assert.Null(savedConfig.Context.EnvironmentName);
    }

    [Fact]
    public void Run_ApiFailure_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Delete(It.IsAny<DeleteEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForFailure(HttpStatusCode.BadRequest));

        // Act
        var result = App.Run("prod");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
    }

    private Config ServerConfig()
    {
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

        return config;
    }
}