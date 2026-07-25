using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Environments;
using Pulse.App.Dto.Common;
using Pulse.Cli.Commands.Environment;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Environment;

public sealed class EnvAddCommandTests : CliTests
{
    public EnvAddCommandTests()
    {
        App.SetDefaultCommand<EnvAddCommand>();
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
        var result = App.Run("production");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No application selected", result.Output);
    }

    [Fact]
    public void Run_Name_ShouldAddEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Create(It.IsAny<CreateEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(new IdentityDto { Id = "1" }, HttpStatusCode.OK));

        // Act
        var result = App.Run("production");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        CtrlApiClient.EnvironmentsMock.Verify(x => x.Create(
            It.Is<CreateEnvironmentRequest>(r => r.EnvironmentName == "production" && r.OrganizationName == "myorg" && r.ApplicationName == "myapp"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Run_Name_ShouldSelectEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        config.SetApplication("myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Create(It.IsAny<CreateEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(new IdentityDto { Id = "1" }, HttpStatusCode.OK));

        // Act
        App.Run("production");

        // Assert
        var savedConfig = ConfigService.Load();
        Assert.Equal("production", savedConfig.Context.EnvironmentName);
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
            .Setup(x => x.Create(It.IsAny<CreateEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForFailure(HttpStatusCode.BadRequest));

        // Act
        var result = App.Run("production");

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