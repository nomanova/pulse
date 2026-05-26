using Microsoft.EntityFrameworkCore;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Common.Database;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }
    
    DbSet<Organization> Organizations { get; }
    
    DbSet<Application> Applications { get; }
    
    DbSet<Environment> Environments { get; }
}