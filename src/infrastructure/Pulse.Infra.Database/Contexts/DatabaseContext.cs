using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Database;

namespace Pulse.Infra.Database.Contexts;

public abstract class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly DatabaseOptions _databaseOptions;
    
    protected DatabaseContext(
        IOptions<DatabaseOptions> databaseOptions,
        DbContextOptions contextOptions) : base(contextOptions)
    {
        _databaseOptions = databaseOptions.Value;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        optionsBuilder
            .EnableSensitiveDataLogging(_databaseOptions.WithSensitiveDataLogging)
            .UseSnakeCaseNamingConvention();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Application
        ApplyConfigurations(modelBuilder, _databaseOptions.Provider);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder, DatabaseProvider provider)
    {
    }
}