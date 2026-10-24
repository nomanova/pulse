using System.Linq;
using System.Threading.Tasks;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Organizations.Commands;
using Pulse.App.Tests.Framework;
using Pulse.Domain.Common.Models.Enums;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Organizations.Commands;

public sealed class CreateOrganizationTests : AppTests
{
    [Fact]
    public async Task Create_WithPermissionsAndValidData_ShouldSucceed()
    {
        // Arrange
        var admin = EnsureAdmin();

        var command = new AddOrganizationCommand
        {
            OrganizationName = "Pulse",
            OrganizationKey = "pulse"
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);
        Assert.NotNull(result.Value.Id);

        var membership = DatabaseContext.Object.Memberships.FirstOrDefault(m => m.Scope == Scope.Organization);
        Assert.NotNull(membership);
        Assert.Equal(admin.User.Id, membership.UserId);
    }

    [Fact]
    public async Task Create_WithDuplicateKey_ShouldReturnKeyInUseError()
    {
        // Arrange
        var admin = EnsureAdmin();
        var organization = EnsureOrganization(admin.User);

        var command = new AddOrganizationCommand
        {
            OrganizationName = organization.Name.Value,
            OrganizationKey = organization.Key.Value
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.True(result.IsError);
        Assert.Contains(ApplicationErrors.KeyInUse, result.Errors);
    }
}