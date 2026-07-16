using System.Threading.Tasks;
using Moq;
using Pulse.App.Handlers.Users.Commands;
using Pulse.App.Tests.Framework;
using Pulse.Domain.Aggregates.Users;
using Xunit;

namespace Pulse.App.Tests.Unit.Handlers.Users.Commands;

public class SignInUserTests : AppTests
{
    [Fact]
    public async Task SignIn_WithValidCredentials_ShouldSucceed()
    {
        // Arrange
        var context = EnsureUser();

        var command = new SignInUserCommand
        {
            Username = context.User.Username.Value,
            Password = context.Password
        };

        // Act
        var result = await Sender.Send(command);

        // Assert
        Assert.False(result.IsError);

        var auth = result.Value;
        Assert.NotNull(auth);
        Assert.Equal(context.User.Id.Value, auth.User.Id);
        
        JwtProvider.Verify(mock => mock.Create(It.IsAny<User>()), Times.Once);
    }
}