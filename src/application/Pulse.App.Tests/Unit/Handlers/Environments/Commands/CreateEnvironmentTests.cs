using System.Threading.Tasks;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Tests.Shared.Builders;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Environments.Commands;

public sealed class CreateEnvironmentTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);

        var application = ApplicationBuilder.New(organization).Build();
        DatabaseContext.AddApplications(application);

        var command = new CreateEnvironmentCommand
        {
            OrganizationName = organization.Name.Value,
            ApplicationName = application.Name.Value,
            EnvironmentName = "test-env"
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value.Id);
    }

    [Fact]
    public async Task Create_WithDuplicateName_ShouldReturnNameInUseError()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);

        var application = ApplicationBuilder.New(organization).Build();
        DatabaseContext.AddApplications(application);

        var environment = Environment.Create("test-env", application);
        DatabaseContext.WithEnvironments(environment);

        var command = new CreateEnvironmentCommand
        {
            OrganizationName = organization.Name.Value,
            ApplicationName = application.Name.Value,
            EnvironmentName = environment.Name.Value
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(ApplicationErrors.NameInUse, result.Errors);
    }
}