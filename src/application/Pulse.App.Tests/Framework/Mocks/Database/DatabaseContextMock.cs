using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Tests.Framework.Mocks.Database;

public static class DatabaseContextMock
{
    public static Mock<IDatabaseContext> Default()
    {
        var mock = new Mock<IDatabaseContext>();

        mock.WithUsers();
        mock.WithOrganizations();

        return mock;
    }

    public static void WithUsers(this Mock<IDatabaseContext> mock, params User[] users)
    {
        mock.Setup(dbContext => dbContext.Users).Returns(ToSet(users).Object);
    }

    public static void WithOrganizations(this Mock<IDatabaseContext> mock, params Organization[] organizations)
    {
        mock.Setup(dbContext => dbContext.Organizations).Returns(ToSet(organizations).Object);
    }

    private static Mock<DbSet<T>> ToSet<T>(T[] items) where T : class
    {
        return items.BuildMock().BuildMockDbSet();
    }
}