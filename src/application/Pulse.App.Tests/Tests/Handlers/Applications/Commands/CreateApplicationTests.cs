using System.Threading.Tasks;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Tests.Shared.Builders;
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
            OrganizationName = context.Organization.Name.Value,
            ApplicationName = "test-app"
        };
        
        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);
        
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Id);
    }
    
    [Fact]
    public async Task Create_WithDuplicateApplicationName_ShouldReturnNameInUseError()
    {
        // Arrange
        var context = EnsureOwnedOrganization();

        var application = ApplicationBuilder.New(context.Organization)
            .WithName("test-app")
            .Build();

        DatabaseContext.AddApplications(application);

        var command = new CreateApplicationCommand
        {
            OrganizationName = context.Organization.Name.Value,
            ApplicationName = application.Name.Value
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(ApplicationErrors.NameInUse, result.Errors);
    }
}