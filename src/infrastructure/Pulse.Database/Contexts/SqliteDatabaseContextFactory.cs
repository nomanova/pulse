using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Throw;

namespace Pulse.Database.Contexts;

public sealed class SqliteDatabaseContextFactory : IDesignTimeDbContextFactory<SqliteDatabaseContext>
{
    /**
     * Add migration
     * dotnet ef migrations add User --context SqliteDatabaseContext --output-dir Migrations/Sqlite -- "Data Source=/Users/dries/Development/nomanova/projects/data/pulse.db;"
     *
     * Update database
     * dotnet ef database update --context SqliteDatabaseContext -- "Data Source=/Users/dries/Development/nomanova/pulse/data/pulse.db;"
     */
    public SqliteDatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = args[0];
        connectionString.ThrowIfNull();

        var databaseOptions = Options.Create(new DatabaseOptions
        {
            ConnectionString = connectionString,
            Provider = DatabaseProvider.Sqlite
        });
        
        var builder = new DbContextOptionsBuilder<SqliteDatabaseContext>();
        builder.Configure(databaseOptions.Value);
        
        return new SqliteDatabaseContext(databaseOptions, builder.Options);
    }
}