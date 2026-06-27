using System.Linq;
using Microsoft.EntityFrameworkCore;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Memberships;
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
        mock.WithMemberships();
        mock.WithApplications();

        return mock;
    }

    extension(Mock<IDatabaseContext> mock)
    {
        public void WithUsers(params User[] users)
        {
            mock.Setup(dbContext => dbContext.Users).Returns(ToSet(users).Object);
        }

        public void AddUsers(params User[] users)
        {
            var allUsers = mock.Object.Users.ToList();
            allUsers.AddRange(users);
        
            mock.Setup(dbContext => dbContext.Users).Returns(ToSet(allUsers.ToArray()).Object);
        }
        
        public void WithOrganizations(params Organization[] organizations)
        {
            mock.Setup(dbContext => dbContext.Organizations).Returns(ToSet(organizations).Object);
        }

        public void WithMemberships(params Membership[] memberships)
        {
            mock.Setup(dbContext => dbContext.Memberships).Returns(ToSet(memberships).Object);
        }
        
        public void WithApplications(params Application[] applications)
        {
            mock.Setup(dbContext => dbContext.Applications).Returns(ToSet(applications).Object);
        }
    }

    private static Mock<DbSet<T>> ToSet<T>(T[] items) where T : class
    {
        return items.BuildMock().BuildMockDbSet();
    }
}