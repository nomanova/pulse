using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.Database.Contexts;

public sealed class SqliteDatabaseContext : DatabaseContext
{
    public SqliteDatabaseContext(
        IOptions<DatabaseOptions> databaseOptions,
        DbContextOptions contextOptions) : base(databaseOptions, contextOptions)
    {
    }
    
    /**
     * EF Core doesn't auto-increment uint row versions on SQLite
     */
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changedEntries = ChangeTracker.Entries<IVersioned>()
            .Where(e => e.State is EntityState.Modified or EntityState.Added);
        
        foreach (var entry in changedEntries)
        {
            entry.Entity.Version++;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}