using System.Threading.Tasks;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Tests.Framework;
using Xunit;

namespace Pulse.App.Tests.Tests.Handlers.Applications.Commands;

public class CreateApplicationTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var context = EnsureOwnedOrganization();
        
        var command = new CreateApplicationCommand
        {
            OrganizationId = context.Organization.Id,
            Name = "Test Application"
        };
        
        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);
        
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Id);
    }
}