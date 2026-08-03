using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Common;
using Pulse.Cli.Commands.Application;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Application;

public sealed class AppAddCommandTests : CliTests
{
    public AppAddCommandTests()
    {
        App.SetDefaultCommand<AppAddCommand>();
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
    public void Run_Name_ShouldAddApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Add(It.IsAny<AddApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(new IdentityDto { Id = "1" }, HttpStatusCode.OK));

        // Act
        var result = App.Run("myapp");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        CtrlApiClient.ApplicationsMock.Verify(x => x.Add(
            It.Is<AddApplicationRequest>(r => r.ApplicationName == "myapp" && r.OrganizationId == "org_1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Run_Name_ShouldSelectApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Add(It.IsAny<AddApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(new IdentityDto { Id = "1" }, HttpStatusCode.OK));

        // Act
        App.Run("myapp");

        // Assert
        var savedConfig = ConfigService.Load();
        Assert.Equal("myapp", savedConfig.Context.Application?.Name);
    }

    [Fact]
    public void Run_ApiFailure_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Add(It.IsAny<AddApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForFailure(HttpStatusCode.BadRequest));

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