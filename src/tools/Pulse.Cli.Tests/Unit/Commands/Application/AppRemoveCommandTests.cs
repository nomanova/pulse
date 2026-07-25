using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.Cli.Commands.Application;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Application;

public sealed class AppRemoveCommandTests : CliTests
{
    public AppRemoveCommandTests()
    {
        App.SetDefaultCommand<AppRemoveCommand>();
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
    public void Run_NoOrganizationSelected_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(new Config());

        // Act
        var result = App.Run("myapp");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No organization selected", result.Output);
    }

    [Fact]
    public void Run_Name_ShouldRemoveApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Delete(It.IsAny<DeleteApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("myapp");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        CtrlApiClient.ApplicationsMock.Verify(x => x.Delete(
            It.Is<DeleteApplicationRequest>(r => r.ApplicationName == "myapp" && r.OrganizationName == "myorg"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Run_SelectedApplication_ShouldClearSelectedApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Delete(It.IsAny<DeleteApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        App.Run("myapp");

        // Assert
        var savedConfig = ConfigService.Load();
        Assert.Null(savedConfig.Context.ApplicationName);
    }

    [Fact]
    public void Run_ApiFailure_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Delete(It.IsAny<DeleteApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForFailure(HttpStatusCode.BadRequest));

        // Act
        var result = App.Run("myapp");

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