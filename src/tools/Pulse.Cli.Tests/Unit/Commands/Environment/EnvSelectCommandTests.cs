using System;
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

public sealed class EnvSelectCommandTests : CliTests
{
    public EnvSelectCommandTests()
    {
        App.SetDefaultCommand<EnvSelectCommand>();
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
    public void Run_NoEnvironmentsFound_ShouldPrintNoEnvironmentsFound()
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
    public void Run_OneEnvironmentFound_ShouldSelectEnvironment()
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
                new() { Id = "1", Name = "prod" }
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
        Assert.Contains("Selected environment 'prod'", result.Output);
        
        var savedConfig = ConfigService.Load();
        Assert.Equal("prod", savedConfig.Context.Environment?.Name);
    }

    [Fact]
    public void Run_MultipleEnvironmentsFound_ShouldPromptAndSelect()
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

        App.Console.Profile.Capabilities.Interactive = true;
        App.Console.Input.PushKey(ConsoleKey.Enter); // Select first choice "prod"

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Selected environment 'prod'", result.Output);

        var savedConfig = ConfigService.Load();
        Assert.Equal("prod", savedConfig.Context.Environment?.Name);
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