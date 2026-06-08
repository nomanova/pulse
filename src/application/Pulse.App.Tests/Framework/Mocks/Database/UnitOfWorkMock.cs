using System.Threading;
using System.Threading.Tasks;
using Moq;
using Pulse.App.Common.Database;

namespace Pulse.App.Tests.Framework.Mocks.Database;

public static class UnitOfWorkMock
{
    public static Mock<IUnitOfWork> Default()
    {
        var mock = new Mock<IUnitOfWork>();

        mock.Setup(m => m.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return mock;
    }
}