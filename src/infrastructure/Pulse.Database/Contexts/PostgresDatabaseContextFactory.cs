using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Options;
using Throw;

namespace Pulse.Database.Contexts;

public sealed class PostgresDatabaseContextFactory : IDesignTimeDbContextFactory<PostgresDatabaseContext>
{
    /**
     * Add migration
     * dotnet ef migrations add Init --context PostgresDatabaseContext --output-dir Migrations/Postgres -- "User ID=pulse;Password=pulse;Host=localhost;Port=5432;Database=pulse;Pooling=true;"
     *
     * Update database
     * dotnet ef database update --context PostgresDatabaseContext -- "User ID=pulse;Password=pulse;Host=localhost;Port=5432;Database=pulse;Pooling=true;"
     *
     * Revert migration
     * dotnet ef database update 'previous migration' -- "User ID=pulse;Password=pulse;Host=localhost;Port=5432;Database=pulse;Pooling=true;"
     *
     * Remove migration
     * dotnet ef migrations remove -- "connection string"
     */
    public PostgresDatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = args[0];
        connectionString.ThrowIfNull();

        var databaseOptions = Options.Create(new DatabaseOptions
        {
            ConnectionString = connectionString,
            Provider = DatabaseProvider.Postgres
        });
        
        var builder = new DbContextOptionsBuilder<PostgresDatabaseContext>();
        builder.Configure(databaseOptions.Value);
        
        return new PostgresDatabaseContext(databaseOptions, builder.Options);
    }
}