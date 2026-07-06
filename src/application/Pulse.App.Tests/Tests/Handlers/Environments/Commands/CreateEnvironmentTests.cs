using System.Threading.Tasks;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Handlers.Environments.Commands;
using Pulse.App.Tests.Framework;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Common.Models.Entities;
using Xunit;

namespace Pulse.App.Tests.Tests.Handlers.Environments.Commands;

public class CreateEnvironmentTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var context = EnsureOwnedOrganization();
        
        var createApplicationCommand = new CreateApplicationCommand
        {
            OrganizationId = context.Organization.Id,
            Name = "Test Application"
        };
        
        var createApplicationResult = await Sender.Send(createApplicationCommand);
        Assert.False(createApplicationResult.IsError);
        
        var application = createApplicationResult.Value;

        var command = new CreateEnvironmentCommand
        {
            OrganizationId = context.Organization.Id,
            ApplicationId = application.Id.AsIdentity<ApplicationId>(),
            Name = "Test Environment"
        };

        // Act
        var result = await Sender.Send(command);
        
        // Assert
        Assert.False(result.IsError);
        
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Id);
    }
}