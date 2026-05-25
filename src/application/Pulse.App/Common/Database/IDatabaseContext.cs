using Microsoft.EntityFrameworkCore;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Common.Database;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }
}