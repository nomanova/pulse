using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Common.Exceptions;
using Pulse.Infra.Database.Configurations;
using Pulse.Infra.Database.Converters;
using Pulse.Infra.Database.Seeders;
using ApplicationId = Pulse.Domain.Aggregates.Applications.ApplicationId;
using Environment = Pulse.Domain.Aggregates.Environments.Environment;
using Event = Pulse.Infra.Database.Messaging.Events.Event;

namespace Pulse.Infra.Database.Contexts;

public abstract class DatabaseContext : DbContext, IDatabaseContext
{
    private readonly DatabaseOptions _databaseOptions;
 
    public DbSet<Event> Events { get; init; }
    
    public DbSet<User> Users { get; init; }
    
    public DbSet<Organization> Organizations { get; init; }
    
    public DbSet<Application> Applications { get; init; }
    
    public DbSet<Environment> Environments { get; init; }
    
    public DbSet<Membership> Memberships { get; init; }
    
    public DbSet<Role> Roles { get; init; }
    
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
        
        BuiltInRoleSeeder.Seed(serviceProvider).Wait();
        DefaultOrgSeeder.Seed(serviceProvider).Wait();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<UserId>().HaveConversion<EntityIdConverter<UserId>>();
        configurationBuilder.Properties<OrganizationId>().HaveConversion<EntityIdConverter<OrganizationId>>();
        configurationBuilder.Properties<ApplicationId>().HaveConversion<EntityIdConverter<ApplicationId>>();
        configurationBuilder.Properties<EnvironmentId>().HaveConversion<EntityIdConverter<EnvironmentId>>();
        
        configurationBuilder.Properties<MembershipId>().HaveConversion<EntityIdConverter<MembershipId>>();
        configurationBuilder.Properties<RoleId>().HaveConversion<EntityIdConverter<RoleId>>();
    }

    private static void ApplyConfigurations(ModelBuilder modelBuilder, DatabaseProvider provider)
    {
        modelBuilder.ApplyConfiguration(new EventConfiguration(provider));
        
        modelBuilder.ApplyConfiguration(new UserConfiguration(provider));
        modelBuilder.ApplyConfiguration(new OrganizationConfiguration(provider));
        modelBuilder.ApplyConfiguration(new ApplicationConfiguration(provider));
        modelBuilder.ApplyConfiguration(new EnvironmentConfiguration(provider));
        
        modelBuilder.ApplyConfiguration(new MembershipConfiguration(provider));
        modelBuilder.ApplyConfiguration(new RoleConfiguration(provider));
    }
}