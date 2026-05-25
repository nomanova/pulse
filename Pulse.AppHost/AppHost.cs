using System;
using System.Threading;
using System.Threading.Tasks;
using Aspire.Hosting;

namespace Pulse.AppHost;

public static class AppHost
{
    private const int ExitSuccess = 0;

    private const string DbProviderSqlite = "Sqlite";
    private const string DbProviderPostgres = "Postgres";

    public static int Main(string[] args)
    {
        return MainAsync(args).GetAwaiter().GetResult();
    }
    
    private static async Task<int> MainAsync(string[] args, CancellationToken cancellationToken = default)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var dbProviderParameter = builder.AddParameter("DatabaseProvider");
        var dbProvider = await dbProviderParameter.Resource.GetValueAsync(cancellationToken);

        switch (dbProvider)
        {
            case DbProviderSqlite:
                await builder.WithSqlite(cancellationToken);
                break;
            case DbProviderPostgres:
                builder.WithPostgres();
                break;
            default:
                throw new NotImplementedException(dbProvider);
        }

        await builder.Build().RunAsync(cancellationToken);
        return ExitSuccess;
    }
}

public static class BuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public async Task WithSqlite(CancellationToken cancellationToken = default)
        {
            var dbPathParameter = builder.AddParameter("DatabasePath");
            var dbPath = await dbPathParameter.Resource.GetValueAsync(cancellationToken);
            
            var sqlite = builder.AddSqlite("sqlite", dbPath, "pulse.db");

            builder
                .AddProject<Projects.Pulse_Api>("api")
                .WithReference(sqlite)
                .WithEnvironment("Database__Provider", "Sqlite")
                .WithEnvironment("Database__ConnectionString", sqlite.Resource.ConnectionStringExpression);
        }

        public void WithPostgres()
        {
            var dbUsernameParameter = builder.AddParameter("DatabaseUsername");
            var dbPasswordParameter = builder.AddParameter("DatabasePassword");

            var postgres = builder.AddPostgres("postgres", dbUsernameParameter, dbPasswordParameter, 5432);

            const string databaseName = "pulse";

            const string creationScript = $"""
                                           CREATE USER {databaseName} WITH PASSWORD {databaseName};
                                           CREATE DATABASE {databaseName};
                                           GRANT ALL PRIVILEGES ON DATABASE {databaseName} TO {databaseName};
                                           """;

            var postgresDatabase = postgres.AddDatabase(databaseName)
                .WithCreationScript(creationScript);

            builder
                .AddProject<Projects.Pulse_Api>("api")
                .WithReference(postgresDatabase)
                .WaitFor(postgresDatabase)
                .WithEnvironment("Database__Provider", "Postgres")
                .WithEnvironment("Database__ConnectionString", postgresDatabase.Resource.ConnectionStringExpression);
        }
    }
}