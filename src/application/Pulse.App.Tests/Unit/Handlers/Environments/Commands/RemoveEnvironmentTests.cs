using System.Threading.Tasks;
using ErrorOr;
using Moq;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Common.Services;
using Pulse.Tests.Shared.Builders;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Environments.Commands;

public sealed class RemoveEnvironmentTests : AppTests
{
    [Fact]
    public async Task Remove_WithNonPrimaryEnvironment_ShouldSucceed()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);

        var application = ApplicationBuilder.New(organization).Build();
        DatabaseContext.AddApplications(application);

        var primary = Environment.Create("Production", "production", application, true);
        var staging = Environment.Create("Staging", "staging", application);
        DatabaseContext.WithEnvironments(primary, staging);

        var command = new RemoveEnvironmentCommand { EnvironmentId = staging.Id };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);
        DatabaseContext.Verify(context => context.Environments.Remove(staging), Times.Once);
        UnitOfWork.Verify(unitOfWork => unitOfWork.Commit(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remove_WithPrimaryEnvironment_ShouldReturnRemovePrimaryError()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);

        var application = ApplicationBuilder.New(organization).Build();
        DatabaseContext.AddApplications(application);

        var primary = Environment.Create("Production", "production", application, true);
        DatabaseContext.WithEnvironments(primary);

        var command = new RemoveEnvironmentCommand { EnvironmentId = primary.Id };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(ApplicationErrors.Environment.RemovePrimary, result.Errors);
        DatabaseContext.Verify(context => context.Environments.Remove(It.IsAny<Environment>()), Times.Never);
    }

    [Fact]
    public async Task Remove_WithUnknownEnvironment_ShouldReturnNotFoundError()
    {
        // Arrange
        EnsureAdmin();

        var command = new RemoveEnvironmentCommand { EnvironmentId = IdentityProvider.New<EnvironmentId>() };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Equal(ErrorType.NotFound, result.FirstError.Type);
    }
}
