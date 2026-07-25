using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Applications;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.Cli.Commands.Application;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Application;

public sealed class AppSelectCommandTests : CliTests
{
    public AppSelectCommandTests()
    {
        App.SetDefaultCommand<AppSelectCommand>();
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
    public void Run_NoApplicationsFound_ShouldPrintNoApplicationsFound()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
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
    public void Run_OneApplicationFound_ShouldSelectApplication()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
        ConfigService.UseConfig(config);

        var searchResult = new PagedSearchResultDto<ApplicationDto>
        {
            Entities = new List<ApplicationDto>
            {
                new() { Id = "1", Name = "app1" }
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
        Assert.Contains("Selected application 'app1'", result.Output);
        
        var savedConfig = ConfigService.Load();
        Assert.Equal("app1", savedConfig.Context.ApplicationName);
    }

    [Fact]
    public void Run_MultipleApplicationsFound_ShouldPromptAndSelect()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("myorg");
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

        App.Console.Profile.Capabilities.Interactive = true;
        App.Console.Input.PushKey(ConsoleKey.Enter); // Select first choice "app1"

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Selected application 'app1'", result.Output);

        var savedConfig = ConfigService.Load();
        Assert.Equal("app1", savedConfig.Context.ApplicationName);
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