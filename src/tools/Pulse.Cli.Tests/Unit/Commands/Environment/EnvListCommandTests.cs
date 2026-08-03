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

public sealed class EnvListCommandTests : CliTests
{
    public EnvListCommandTests()
    {
        App.SetDefaultCommand<EnvListCommand>();
    }

    [Fact]
    public void Run_NoApplicationSelected_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No application selected", result.Output);
    }

    [Fact]
    public void Run_NoEnvironments_ShouldPrintNoEnvironmentsFound()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<EnvironmentDto>
        {
            Entities = new List<EnvironmentDto>(),
            HasNext = false
        };

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("No environments found", result.Output);
    }

    [Fact]
    public void Run_WithEnvironments_ShouldPrintEnvironments()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<EnvironmentDto>
        {
            Entities = new List<EnvironmentDto>
            {
                new() { Id = "1", Name = "prod" },
                new() { Id = "2", Name = "stage" }
            },
            HasNext = false
        };

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("prod", result.Output);
        Assert.Contains("stage", result.Output);
    }

    [Fact]
    public void Run_SelectedEnvironment_ShouldHighlightSelectedEnvironment()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1", "myapp");
        config.SetEnvironment("env_1","prod");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<EnvironmentDto>
        {
            Entities = new List<EnvironmentDto>
            {
                new() { Id = "1", Name = "prod" },
                new() { Id = "2", Name = "stage" }
            },
            HasNext = false
        };

        CtrlApiClient.EnvironmentsMock
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("*", result.Output);
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
            .Setup(x => x.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForFailure(HttpStatusCode.InternalServerError));

        // Act
        var result = App.Run();

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