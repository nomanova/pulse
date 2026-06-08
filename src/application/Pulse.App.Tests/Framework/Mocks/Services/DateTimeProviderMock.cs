using System;
using Moq;
using Pulse.App.Common.Services.Interfaces;

namespace Pulse.App.Tests.Framework.Mocks.Services;

public static class DateTimeProviderMock
{
    public static Mock<IDateTimeProvider> Default()
    {
        var mock = new Mock<IDateTimeProvider>();
        return mock;
    }

    public static void UtcNow(this Mock<IDateTimeProvider> mock, DateTime value)
    {
        mock.Setup(m => m.UtcNow).Returns(value);
    }
}