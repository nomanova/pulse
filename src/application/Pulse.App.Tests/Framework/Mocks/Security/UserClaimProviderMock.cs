using Moq;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Tests.Framework.Mocks.Security;

public static class UserClaimProviderMock
{
    public static Mock<IUserClaimProvider> Default()
    {
        var mock = new Mock<IUserClaimProvider>();
        return mock;
    }

    public static Mock<IUserClaimProvider> For(User user)
    {
        var mock = new Mock<IUserClaimProvider>();
        AddUserClaims(mock, user);
        return mock;
    }

    private static void AddUserClaims(Mock<IUserClaimProvider> mock, User user)
    {
        mock.Setup(m => m.Id).Returns(() => user.Id.Value);
        mock.Setup(m => m.SecurityStamp).Returns(() => user.SecurityStamp.Value);
    }
}