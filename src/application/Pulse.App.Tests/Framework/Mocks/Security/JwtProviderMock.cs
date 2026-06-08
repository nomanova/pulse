using Moq;
using Pulse.App.Common.Security.Interfaces;

namespace Pulse.App.Tests.Framework.Mocks.Security;

public static class JwtProviderMock
{
    public static Mock<IJwtProvider> Default()
    {
        return new Mock<IJwtProvider>();
    }
}