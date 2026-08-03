using System.Collections.Generic;
using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Environments;
using Pulse.App.Dto.Common;
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
        config.SetOrganization("org_1", "myorg");
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
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<EnvironmentDto>
        {
            Entities = new List<EnvironmentDto> { new() { Id = "env_1", Name = "prod" } },
            HasNext = false
        };

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("prod");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        CtrlApiClient.EnvironmentsMock.Verify(x => x.Remove(
            It.Is<RemoveEnvironmentRequest>(r => r.EnvironmentId == "env_1"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Run_SelectedEnvironment_ShouldClearSelectedEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        config.SetEnvironment("env_1","prod");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<EnvironmentDto>
        {
            Entities = new List<EnvironmentDto> { new() { Id = "env_1", Name = "prod" } },
            HasNext = false
        };

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        App.Run("prod");

        // Assert
        var savedConfig = ConfigService.Load();
        Assert.Null(savedConfig.Context.Environment?.Name);
    }

    [Fact]
    public void Run_ApiFailure_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        ConfigService.UseConfig(config);

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()))
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