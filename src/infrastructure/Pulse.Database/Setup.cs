using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Database;
using Pulse.Database.Contexts;
using Pulse.Database.Outbox;
using Throw;

namespace Pulse.Database;

public static class Setup
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAndValidate<DatabaseOptions>(DatabaseOptions.Section, configuration);
        
        var databaseOptions = configuration.GetSection(DatabaseOptions.Section).Get<DatabaseOptions>();
        databaseOptions.ThrowIfNull();
        
        switch (databaseOptions.Provider)
        {
            case DatabaseProvider.Postgres:
                services.AddDbContext<PostgresDatabaseContext>(builder => { builder.Configure(databaseOptions); });
                break;
            case DatabaseProvider.Sqlite:
                services.AddDbContext<SqliteDatabaseContext>(builder => { builder.Configure(databaseOptions); });
                services.AddHostedService<MigrateDatabaseHostedService<SqliteDatabaseContext>>();
                break;
            default:
                throw new NotImplementedException();
        }
        
        services.AddScoped<DatabaseContext>(provider =>
        {
            return databaseOptions.Provider switch
            {
                DatabaseProvider.Postgres => provider.GetRequiredService<PostgresDatabaseContext>(),
                DatabaseProvider.Sqlite => provider.GetRequiredService<SqliteDatabaseContext>(),
                _ => throw new NotImplementedException()
            };
        });
        
        services.AddScoped<IDatabaseContext>(provider =>
            provider.GetRequiredService<DatabaseContext>());

        services.AddOutbox(configuration);

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}