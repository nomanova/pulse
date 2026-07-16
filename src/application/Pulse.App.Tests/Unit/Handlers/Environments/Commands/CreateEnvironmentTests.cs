using System.Threading.Tasks;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Tests.Shared.Builders;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Environments.Commands;

public class CreateEnvironmentTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var context = EnsureOwnedOrganization();

        var application = ApplicationBuilder.New(context.Organization).Build();
        DatabaseContext.AddApplications(application);

        var command = new CreateEnvironmentCommand
        {
            OrganizationName = context.Organization.Name.Value,
            ApplicationName = application.Name.Value,
            EnvironmentName = "test-env"
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);

        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Id);
    }

    [Fact]
    public async Task Create_WithDuplicateEnvironmentName_ShouldReturnNameInUseError()
    {
        // Arrange
        var context = EnsureOwnedOrganization();

        var application = ApplicationBuilder.New(context.Organization).Build();
        DatabaseContext.AddApplications(application);

        var environment = Environment.Create("test-env", application);
        DatabaseContext.WithEnvironments(environment);

        var command = new CreateEnvironmentCommand
        {
            OrganizationName = context.Organization.Name.Value,
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