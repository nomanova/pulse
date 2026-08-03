using System.Collections.Generic;
using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.Cli.Commands.Application;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Application;

public sealed class AppListCommandTests : CliTests
{
    public AppListCommandTests()
    {
        App.SetDefaultCommand<AppListCommand>();
    }

    [Fact]
    public void Run_NoOrganizationSelected_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(new Config());

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No organization selected", result.Output);
    }

    [Fact]
    public void Run_NoApplications_ShouldPrintNoApplicationsFound()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<ApplicationDto>
        {
            Entities = new List<ApplicationDto>(),
            HasNext = false
        };

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Search(It.IsAny<SearchApplicationsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<ApplicationDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("No applications found", result.Output);
    }

    [Fact]
    public void Run_WithApplications_ShouldPrintApplications()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<ApplicationDto>
        {
            Entities = new List<ApplicationDto>
            {
                new() { Id = "1", Name = "app1" },
                new() { Id = "2", Name = "app2" }
            },
            HasNext = false
        };

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Search(It.IsAny<SearchApplicationsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<ApplicationDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("app1", result.Output);
        Assert.Contains("app2", result.Output);
    }

    [Fact]
    public void Run_SelectedApplication_ShouldHighlightSelectedApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        config.SetApplication("app_1","app1");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<ApplicationDto>
        {
            Entities = new List<ApplicationDto>
            {
                new() { Id = "1", Name = "app1" },
                new() { Id = "2", Name = "app2" }
            },
            HasNext = false
        };

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Search(It.IsAny<SearchApplicationsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<ApplicationDto>>.ForSuccess(searchResult, HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("*", result.Output); // Spectre.Console grey *
    }

    [Fact]
    public void Run_ApiFailure_ShouldFail()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "myorg");
        ConfigService.UseConfig(config);

        CtrlApiClient.ApplicationsMock
            .Setup(x => x.Search(It.IsAny<SearchApplicationsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<ApplicationDto>>.ForFailure(HttpStatusCode.InternalServerError));

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