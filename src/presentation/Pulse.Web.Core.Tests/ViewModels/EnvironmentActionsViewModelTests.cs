using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Client.Services.Interfaces;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Applications.Environments;
using Xunit;

namespace Pulse.Web.Core.Tests.ViewModels;

public class EnvironmentActionsViewModelTests
{
    private const string ApplicationId = "app-1";

    private static readonly ObjectIdentity Production = new("env-prod", "Production", "production", true);
    private static readonly ObjectIdentity Staging = new("env-staging", "Staging", "staging");

    private readonly Mock<IContextProvider> _contextProvider = new();
    private readonly Mock<IEnvironmentsService> _environments = new();
    private readonly Mock<IDialogManager> _dialogManager = new();
    private readonly Mock<IFlagManager> _flagManager = new();
    private readonly EnvironmentActionsViewModel _sut;

    public EnvironmentActionsViewModelTests()
    {
        var apiClient = new Mock<ICtrlApiClient>();
        apiClient.SetupGet(client => client.Environments).Returns(_environments.Object);

        _sut = new EnvironmentActionsViewModel(
            new WeakReferenceMessenger(),
            _contextProvider.Object,
            apiClient.Object,
            _dialogManager.Object,
            _flagManager.Object)
        {
            ApplicationId = ApplicationId
        };
    }

    private void SetupRemove(HttpStatusCode statusCode)
    {
        _environments
            .Setup(service => service.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(statusCode == HttpStatusCode.OK
                ? ApiResult.ForSuccess(statusCode)
                : ApiResult.ForFailure(statusCode));
    }

    [Fact]
    public async Task Remove_WithPrimaryEnvironment_ShouldShowInfoAndNotRemove()
    {
        // Arrange
        _contextProvider.SetupGet(provider => provider.CurrentEnvironment).Returns(Production);

        // Act
        await _sut.RemoveCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_sut.IsRemoving);
        _dialogManager.Verify(
            dialog => dialog.ShowInfo(EnvironmentActionsViewModel.PrimaryTitle,
                EnvironmentActionsViewModel.PrimaryMessage), Times.Once);
        _environments.Verify(
            service => service.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _contextProvider.Verify(provider => provider.RemoveEnvironment(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Remove_WhenConfirmationDeclined_ShouldNotRemove()
    {
        // Arrange
        _contextProvider.SetupGet(provider => provider.CurrentEnvironment).Returns(Staging);
        _dialogManager
            .Setup(dialog => dialog.GetRemoveConfirmation("environment", Staging.Name,
                EnvironmentActionsViewModel.RemoveWarning))
            .ReturnsAsync(false);

        // Act
        await _sut.RemoveCommand.ExecuteAsync(null);

        // Assert
        _environments.Verify(
            service => service.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _contextProvider.Verify(provider => provider.RemoveEnvironment(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Remove_WhenConfirmed_ShouldRemoveAndSwitchEnvironment()
    {
        // Arrange
        _contextProvider.SetupGet(provider => provider.CurrentEnvironment).Returns(Staging);
        _dialogManager
            .Setup(dialog => dialog.GetRemoveConfirmation("environment", Staging.Name,
                EnvironmentActionsViewModel.RemoveWarning))
            .ReturnsAsync(true);
        SetupRemove(HttpStatusCode.OK);

        // Act
        await _sut.RemoveCommand.ExecuteAsync(null);

        // Assert
        _environments.Verify(
            service => service.Remove(
                It.Is<RemoveEnvironmentRequest>(request => request.EnvironmentId == Staging.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _contextProvider.Verify(provider => provider.RemoveEnvironment(ApplicationId, Staging.Id), Times.Once);
    }

    [Fact]
    public async Task Remove_WhenApiFails_ShouldRaiseErrorAndKeepEnvironment()
    {
        // Arrange
        _contextProvider.SetupGet(provider => provider.CurrentEnvironment).Returns(Staging);
        _dialogManager
            .Setup(dialog => dialog.GetRemoveConfirmation("environment", Staging.Name,
                EnvironmentActionsViewModel.RemoveWarning))
            .ReturnsAsync(true);
        SetupRemove(HttpStatusCode.InternalServerError);

        // Act
        await _sut.RemoveCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_sut.IsRemoving);
        _flagManager.Verify(flags => flags.RaiseError(It.IsAny<string>()), Times.Once);
        _contextProvider.Verify(provider => provider.RemoveEnvironment(It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Remove_WhileRequestPending_ShouldBeRemovingAndIgnoreSecondInvocation()
    {
        // Arrange
        _contextProvider.SetupGet(provider => provider.CurrentEnvironment).Returns(Staging);
        _dialogManager
            .Setup(dialog => dialog.GetRemoveConfirmation("environment", Staging.Name,
                EnvironmentActionsViewModel.RemoveWarning))
            .ReturnsAsync(true);

        var completion = new TaskCompletionSource<ApiResult>();
        _environments
            .Setup(service => service.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()))
            .Returns(completion.Task);

        // Act
        var first = _sut.RemoveCommand.ExecuteAsync(null);
        var isRemovingWhilePending = _sut.IsRemoving;
        await _sut.RemoveCommand.ExecuteAsync(null);

        completion.SetResult(ApiResult.ForSuccess(HttpStatusCode.OK));
        await first;

        // Assert
        Assert.True(isRemovingWhilePending);
        Assert.False(_sut.IsRemoving);
        _environments.Verify(
            service => service.Remove(It.IsAny<RemoveEnvironmentRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
