using System.Threading.Tasks;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Applications.Commands;
using Pulse.App.Tests.Framework;
using Pulse.App.Tests.Framework.Mocks.Database;
using Pulse.Tests.Shared.Builders;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Applications.Commands;

public sealed class CreateApplicationTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);
        
        var command = new AddApplicationCommand
        {
            OrganizationId = organization.Id,
            ApplicationName = "test-app"
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

        var application = ApplicationBuilder.New(organization)
            .WithName("test-app")
            .Build();

        DatabaseContext.AddApplications(application);

        var command = new AddApplicationCommand
        {
            OrganizationId = organization.Id,
            ApplicationName = application.Name.Value
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(ApplicationErrors.NameInUse, result.Errors);
    }
}