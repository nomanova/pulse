using System.Threading;
using Moq;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Tests.Framework.Mocks.Security;

public static class CachedUserProviderMock
{
    public static Mock<ICachedUserProvider> Default()
    {
        var mock = new Mock<ICachedUserProvider>();

        mock.Setup(m => m.Get(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException());

        return mock;
    }

    public static Mock<ICachedUserProvider> For(User user)
    {
        var mock = new Mock<ICachedUserProvider>();

        mock.Setup(m => m.Get(It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        return mock;
    }
}