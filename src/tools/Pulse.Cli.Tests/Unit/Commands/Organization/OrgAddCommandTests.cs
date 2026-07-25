using System.Net;
using System.Threading;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Contract.Organizations;
using Pulse.Api.Shared.Contract;
using Pulse.App.Dto.Common;
using Pulse.Cli.Commands.Organization;
using Pulse.Cli.Models;
using Pulse.Cli.Tests.Framework;
using Xunit;

namespace Pulse.Cli.Tests.Unit.Commands.Organization;

public sealed class OrgAddCommandTests : CliTests
{
    public OrgAddCommandTests()
    {
        App.SetDefaultCommand<OrgAddCommand>();
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
            organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public void Run_Name_ShouldCreateOrganization()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(null, HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);

        CtrlApiClient.OrganizationsMock.Verify(
            organizations => organizations.Create(
                It.Is<CreateOrganizationRequest>(request =>
                    request.OrganizationName == "default"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public void Run_Name_ShouldSelectAddedOrganization()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(null, HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("default", ConfigService.SavedConfig.Context.OrganizationName);
    }

    [Fact]
    public void Run_Name_ShouldSaveConfig()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(null, HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Equal("default", ConfigService.SavedConfig.Context.OrganizationName);
    }

    [Fact]
    public void Run_Name_ShouldPrintOrganizationAdded()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(null, HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Organization 'default' added", result.Output);
    }

    [Fact]
    public void Run_Name_ShouldPrintSelectedOrganization()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForSuccess(null, HttpStatusCode.OK));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Success, result.ExitCode);
        Assert.Contains("Selected organization 'default'", result.Output);
    }

    [Fact]
    public void Run_CreateOrganizationFails_ShouldFail()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForFailure(
                HttpStatusCode.Conflict,
                problem: new Problem
                {
                    Code = "Organizations.AlreadyExists",
                    Description = "Organization already exists"
                }));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Contains("Organization already exists", result.Output);
    }

    [Fact]
    public void Run_CreateOrganizationFails_ShouldNotSelectOrganization()
    {
        // Arrange
        ConfigService.UseConfig(ServerConfig());

        CtrlApiClient.OrganizationsMock
            .Setup(organizations => organizations.Create(
                It.IsAny<CreateOrganizationRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<IdentityDto>.ForFailure(
                HttpStatusCode.Conflict,
                problem: new Problem
                {
                    Code = "Organizations.AlreadyExists",
                    Description = "Organization already exists"
                }));

        // Act
        var result = App.Run("default");

        // Assert
        Assert.Equal(Exit.Error, result.ExitCode);
        Assert.Null(ConfigService.Config.Context.OrganizationName);
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