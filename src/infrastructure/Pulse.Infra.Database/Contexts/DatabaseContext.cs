using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Common.Exceptions;
using Pulse.Infra.Database.Configurations;
using Pulse.Infra.Database.Converters;
using Pulse.Infra.Database.Seeders;

namespace Pulse.Infra.Database.Contexts;

public abstract class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly DatabaseOptions _databaseOptions;
    
    public DbSet<User> Users { get; init; }
    
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

    public void EnsureMigrated()
    {
        Database.Migrate();
    }
    
    public void EnsureSeeded(IServiceProvider serviceProvider)
    {
        if (!this.HasMigrationsApplied())
        {
            throw new AppException("Not all migrations were applied to the database");
        }
        
        InitSeeder.Seed(serviceProvider).Wait();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<UserId>().HaveConversion<EntityIdConverter<UserId>>();
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder, DatabaseProvider provider)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration(provider));
    }
}