using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
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
        mock.WithEnvironments();

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

        public void AddOrganizations(params Organization[] organizations)
        {
            var allOrganizations = mock.Object.Organizations.ToList();
            allOrganizations.AddRange(organizations);

            mock.Setup(dbContext => dbContext.Organizations).Returns(ToSet(allOrganizations.ToArray()).Object);
        }

        public void WithMemberships(params Membership[] memberships)
        {
            mock.Setup(dbContext => dbContext.Memberships).Returns(ToSet(memberships).Object);
        }

        public void AddMemberships(params Membership[] memberships)
        {
            var allMemberships = mock.Object.Memberships.ToList();
            allMemberships.AddRange(memberships);

            mock.Setup(dbContext => dbContext.Memberships).Returns(ToSet(allMemberships.ToArray()).Object);
        }

        public void WithApplications(params Application[] applications)
        {
            mock.Setup(dbContext => dbContext.Applications).Returns(ToSet(applications).Object);
        }

        public void AddApplications(params Application[] applications)
        {
            var allApplications = mock.Object.Applications.ToList();
            allApplications.AddRange(applications);

            mock.Setup(dbContext => dbContext.Applications).Returns(ToSet(allApplications.ToArray()).Object);
        }

        public void WithEnvironments(params Environment[] environments)
        {
            mock.Setup(dbContext => dbContext.Environments).Returns(ToSet(environments).Object);
        }
    }

    private static Mock<DbSet<T>> ToSet<T>(params T[] items) where T : class
    {
        var backingStore = new List<T>(items);
        var dbSet = backingStore.BuildMock().BuildMockDbSet();

        dbSet.Setup(set => set.Add(It.IsAny<T>()))
            .Callback<T>(backingStore.Add)
            .Returns((EntityEntry<T>)null!);

        dbSet.Setup(set => set.AddRange(It.IsAny<IEnumerable<T>>()))
            .Callback<IEnumerable<T>>(backingStore.AddRange);

        dbSet.Setup(set => set.AddRange(It.IsAny<T[]>()))
            .Callback<T[]>(backingStore.AddRange);

        dbSet.Setup(set => set.Remove(It.IsAny<T>()))
            .Callback<T>(entity => backingStore.Remove(entity))
            .Returns((EntityEntry<T>)null!);

        return dbSet;
    }
}