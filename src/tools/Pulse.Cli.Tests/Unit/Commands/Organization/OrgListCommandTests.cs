using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Organizations;
using Pulse.Cli.Commands.Organization;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Organization;

public sealed class OrgListCommandTests : CliTests
{
    public OrgListCommandTests()
    {
        App.SetDefaultCommand<OrgListCommand>();
    }

    [Fact]
    public void Run_NoServerSelected_ShouldFail()
    {
        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("No server selected", result.Output);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void Run_ShouldSearchOrganizations()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>(),
                HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.Is<PagedSearchRequest>(request =>
                    request.Query == null &&
                    request.PageSize == Constants.DefaultPageLimit &&
                    request.LastId == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_Query_ShouldSearchOrganizationsByQuery()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>(),
                HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.Is<PagedSearchRequest>(request =>
                    request.Query == "default" &&
                    request.PageSize == Constants.DefaultPageLimit &&
                    request.LastId == null),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_Limit_ShouldSearchOrganizationsWithLimit()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>(),
                HttpStatusCode.OK));

        // Act
        var result = App.Run("--limit", "10");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.Is<PagedSearchRequest>(request =>
                    request.PageSize == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_Cursor_ShouldSearchOrganizationsAfterCursor()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>(),
                HttpStatusCode.OK));

        // Act
        var result = App.Run("--cursor", "org_1");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.Is<PagedSearchRequest>(request =>
                    request.LastId == "org_1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_NoOrganizations_ShouldPrintNoOrganizationsFound()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>(),
                HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("No organizations found", result.Output);
    }

    [Fact]
    public void Run_OrganizationsFound_ShouldPrintOrganizations()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>
                {
                    Entities =
                    [
                        new OrganizationDto
                        {
                            Id = "org_1",
                            Name = "default"
                        },
                        new OrganizationDto
                        {
                            Id = "org_2",
                            Name = "production"
                        }
                    ]
                },
                HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("default", result.Output);
        Assert.Contains("production", result.Output);
    }

    [Fact]
    public void Run_SelectedOrganizationFound_ShouldPrintSelectedOrganizationMarker()
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
                new PagedSearchResultDto<OrganizationDto>
                {
                    Entities =
                    [
                        new OrganizationDto
                        {
                            Id = "org_1",
                            Name = "default"
                        }
                    ]
                },
                HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("*", result.Output);
        Assert.Contains("default", result.Output);
    }

    [Fact]
    public void Run_HasNextPage_ShouldPrintFetchMoreInstructions()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>
                {
                    HasNext = true,
                    Entities =
                    [
                        new OrganizationDto
                        {
                            Id = "org_1",
                            Name = "default"
                        }
                    ]
                },
                HttpStatusCode.OK));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Use option `-c org_1` to fetch more results", result.Output);
    }

    [Fact]
    public void Run_All_ShouldFetchAllPages()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .SetupSequence(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>
                {
                    HasNext = true,
                    Entities =
                    [
                        new OrganizationDto
                        {
                            Id = "org_1",
                            Name = "default"
                        }
                    ]
                },
                HttpStatusCode.OK))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForSuccess(
                new PagedSearchResultDto<OrganizationDto>
                {
                    Entities =
                    [
                        new OrganizationDto
                        {
                            Id = "org_2",
                            Name = "production"
                        }
                    ]
                },
                HttpStatusCode.OK));

        // Act
        var result = App.Run("--all");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("default", result.Output);
        Assert.Contains("production", result.Output);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Search(
                It.Is<PagedSearchRequest>(request =>
                    request.LastId == "org_1"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_SearchOrganizationsFails_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Search(
                It.IsAny<PagedSearchRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<OrganizationDto>>.ForFailure(
                HttpStatusCode.InternalServerError,
                problem: new Problem
                {
                    Code = "Organizations.SearchFailed",
                    Description = "Search organizations failed"
                }));

        // Act
        var result = App.Run();

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("Search organizations failed", result.Output);
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
}