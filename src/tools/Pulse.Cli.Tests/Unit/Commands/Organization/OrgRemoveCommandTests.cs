using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Pulse.Cli.Commands.Organization;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Organization;

public sealed class OrgRemoveCommandTests : CliTests
{
    public OrgRemoveCommandTests()
    {
        App.SetDefaultCommand<OrgRemoveCommand>();
    }

    [Fact]
    public void Run_NoArguments_ShouldPrintInstructions()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Instructions, result.ExitCode);
        Assert.Contains("ARGUMENTS", result.Output);
    }

    [Fact]
    public void Run_NoServerSelected_ShouldFail()
    {
        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No server selected", result.Output);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
    [Fact]
    public void Run_Name_ShouldSaveConfig()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                OrganizationSearchResult(),
                HttpStatusCode.OK));
        
        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.NotNull(ConfigService.SavedConfig);
    }

    [Fact]
    public void Run_Name_ShouldPrintOrganizationRemoved()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                OrganizationSearchResult(),
                HttpStatusCode.OK));
        
        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Organization 'default' removed", result.Output);
    }

    [Fact]
    public void Run_SelectedOrganization_ShouldClearSelectedOrganization()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1","default");

        ConfigService.UseConfig(config);

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                OrganizationSearchResult(),
                HttpStatusCode.OK));
        
        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Null(ConfigService.SavedConfig.Context.Organization);
    }

    [Fact]
    public void Run_UnselectedOrganization_ShouldKeepSelectedOrganization()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1","production");

        ConfigService.UseConfig(config);

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                OrganizationSearchResult(),
                HttpStatusCode.OK));
        
        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForSuccess(HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("production", ConfigService.SavedConfig.Context.Organization?.Name);
    }

    [Fact]
    public void Run_DeleteOrganizationFails_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                OrganizationSearchResult(),
                HttpStatusCode.OK));
        
        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForFailure(
                HttpStatusCode.NotFound,
                problem: new Problem
                {
                    Code = "Organizations.NotFound",
                    Description = "Organization not found"
                }));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("Organization not found", result.Output);
    }

    [Fact]
    public void Run_DeleteOrganizationFails_ShouldNotClearSelectedOrganization()
    {
        // Arrange
        var config = ServerConfig();
        config.SetOrganization("org_1", "default");

        ConfigService.UseConfig(config);

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Remove(
                It.IsAny<RemoveOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResult.ForFailure(
                HttpStatusCode.NotFound,
                problem: new Problem
                {
                    Code = "Organizations.NotFound",
                    Description = "Organization not found"
                }));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Equal("default", ConfigService.Config.Context.Organization?.Name);
    }

    private static Config ServerConfig()
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
    
    private static PagedSearchResultDto<OrganizationDto> OrganizationSearchResult(
        string id = "org_1",
        string name = "default")
    {
        return new PagedSearchResultDto<OrganizationDto>
        {
            Entities =
            [
                new OrganizationDto
                {
                    Id = id,
                    Name = name
                }
            ],
            HasNext = false
        };
    }
}