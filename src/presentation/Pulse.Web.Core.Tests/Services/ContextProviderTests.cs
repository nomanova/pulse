using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
using Pulse.App.Dto.Applications;
using Pulse.App.Dto.Common;
using Pulse.App.Dto.Environments;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services;
using Xunit;

namespace Pulse.Web.Core.Tests.Services;

public class ContextProviderTests
{
    private const string ApplicationId = "app-1";

    private static readonly EnvironmentDto Production =
        new() { Id = "env-prod", Name = "Production", Key = "production", IsPrimary = true };

    private static readonly EnvironmentDto Staging =
        new() { Id = "env-staging", Name = "Staging", Key = "staging" };

    private readonly Mock<IApplicationsService> _applications = new();
    private readonly Mock<IEnvironmentsService> _environments = new();
    private readonly ContextProvider _sut;

    public ContextProviderTests()
    {
        var apiClient = new Mock<ICtrlApiClient>();
        apiClient.SetupGet(client => client.Applications).Returns(_applications.Object);
        apiClient.SetupGet(client => client.Environments).Returns(_environments.Object);

        _applications
            .Setup(service => service.Fetch(It.IsAny<FetchApplicationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<ApplicationDto>.ForSuccess(
                new ApplicationDto { Id = ApplicationId, Name = "App", Key = "app" },
                HttpStatusCode.OK));

        SetupEnvironments(Production);

        _sut = new ContextProvider(new WeakReferenceMessenger(), apiClient.Object);
    }

    private void SetupEnvironments(params EnvironmentDto[] environments)
    {
        _environments
            .Setup(service => service.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiDataResult<PagedSearchResultDto<EnvironmentDto>>.ForSuccess(
                new PagedSearchResultDto<EnvironmentDto> { Entities = new List<EnvironmentDto>(environments) },
                HttpStatusCode.OK));
    }

    [Fact]
    public async Task EnsureApplication_WithoutEnvironmentId_ShouldSelectPrimaryEnvironment()
    {
        // Arrange
        SetupEnvironments(Staging, Production);

        // Act
        var result = await _sut.EnsureApplication(ApplicationId);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Production.Id, _sut.CurrentEnvironment?.Id);
    }

    [Fact]
    public async Task EnsureApplication_WithEnvironmentIdAfterSwitchingToNewEnvironment_ShouldRestoreRequestedEnvironment()
    {
        // Arrange (regression: browser back after adding an environment kept showing the newest environment)
        await _sut.EnsureApplication(ApplicationId, Production.Id);
        _sut.AddAndSwitchToEnvironment(ApplicationId, Staging.ToIdentity());
        Assert.Equal(Staging.Id, _sut.CurrentEnvironment?.Id);

        // Act
        var result = await _sut.EnsureApplication(ApplicationId, Production.Id);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Production.Id, _sut.CurrentEnvironment?.Id);
        Assert.Equal(Production.Name, _sut.CurrentEnvironment?.Name);
    }

    [Fact]
    public async Task EnsureApplication_WithCachedEnvironmentId_ShouldNotReloadFromApi()
    {
        // Arrange
        await _sut.EnsureApplication(ApplicationId, Production.Id);
        _sut.AddAndSwitchToEnvironment(ApplicationId, Staging.ToIdentity());

        // Act
        await _sut.EnsureApplication(ApplicationId, Production.Id);

        // Assert
        _applications.Verify(
            service => service.Fetch(It.IsAny<FetchApplicationRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _environments.Verify(
            service => service.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnsureApplication_WithoutEnvironmentIdForLoadedApplication_ShouldKeepCurrentEnvironment()
    {
        // Arrange
        await _sut.EnsureApplication(ApplicationId, Production.Id);
        _sut.AddAndSwitchToEnvironment(ApplicationId, Staging.ToIdentity());

        // Act
        var result = await _sut.EnsureApplication(ApplicationId);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Staging.Id, _sut.CurrentEnvironment?.Id);
    }

    [Fact]
    public async Task EnsureApplication_WithEnvironmentIdNotInCache_ShouldReloadEnvironmentsFromApi()
    {
        // Arrange
        await _sut.EnsureApplication(ApplicationId, Production.Id);
        SetupEnvironments(Production, Staging);

        // Act
        var result = await _sut.EnsureApplication(ApplicationId, Staging.Id);

        // Assert
        Assert.False(result.IsError);
        Assert.Equal(Staging.Id, _sut.CurrentEnvironment?.Id);
        _environments.Verify(
            service => service.Search(It.IsAny<SearchEnvironmentsRequest>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task RemoveEnvironment_WithCurrentEnvironment_ShouldSwitchToPrimaryEnvironment()
    {
        // Arrange
        SetupEnvironments(Production, Staging);
        await _sut.EnsureApplication(ApplicationId, Staging.Id);

        // Act
        _sut.RemoveEnvironment(ApplicationId, Staging.Id);

        // Assert
        Assert.Equal(Production.Id, _sut.CurrentEnvironment?.Id);
        Assert.DoesNotContain(_sut.Environments!, env => env.Id == Staging.Id);
    }

    [Fact]
    public async Task RemoveEnvironment_WithCurrentEnvironment_ShouldSendEnvironmentChangedMessage()
    {
        // Arrange
        var messenger = new WeakReferenceMessenger();
        var received = false;
        messenger.Register<EnvironmentChangedMessage>(this, (_, _) => received = true);

        var apiClient = new Mock<ICtrlApiClient>();
        apiClient.SetupGet(client => client.Applications).Returns(_applications.Object);
        apiClient.SetupGet(client => client.Environments).Returns(_environments.Object);
        var sut = new ContextProvider(messenger, apiClient.Object);

        SetupEnvironments(Production, Staging);
        await sut.EnsureApplication(ApplicationId, Staging.Id);

        // Act
        sut.RemoveEnvironment(ApplicationId, Staging.Id);

        // Assert
        Assert.True(received);
    }

    [Fact]
    public async Task RemoveEnvironment_WithUnknownApplication_ShouldThrow()
    {
        // Arrange
        await _sut.EnsureApplication(ApplicationId);

        // Act & Assert
        Assert.Throws<System.InvalidOperationException>(() => _sut.RemoveEnvironment("other-app", Production.Id));
    }
}
