using System.Threading.Tasks;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Tests.Shared.Builders;
using Xunit;

namespace Pulse.App.Tests.Tests.Handlers.Environments.Commands;

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
}