using System;
using System.Threading;
using System.Threading.Tasks;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;

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

        var dbOnlyParameter = builder.AddParameter("DatabaseOnly");
        var dbOnly =
            bool.TryParse(await dbOnlyParameter.Resource.GetValueAsync(cancellationToken), out var parsedDbOnly) &&
            parsedDbOnly;

        var dbResource = dbProvider switch
        {
            DbProviderSqlite => await builder.WithSqlite(cancellationToken),
            DbProviderPostgres => await builder.WithPostgres(cancellationToken),
            _ => throw new NotImplementedException(dbProvider)
        };

        if (!dbOnly)
        {
            builder
                .AddProject<Projects.Pulse_Api>("api")
                .WithReference(dbResource)
                .WaitFor(dbResource)
                .WithEnvironment("Database__Provider", dbProvider)
                .WithEnvironment("Database__ConnectionString", dbResource.Resource.ConnectionStringExpression);
        }

        await builder.Build().RunAsync(cancellationToken);
        return ExitSuccess;
    }
}

public static class BuilderExtensions
{
    extension(IDistributedApplicationBuilder builder)
    {
        public async Task<IResourceBuilder<IResourceWithConnectionString>> WithSqlite(
            CancellationToken cancellationToken = default)
        {
            var dbPathParameter = builder.AddParameter("DatabasePath");
            var dbPath = await dbPathParameter.Resource.GetValueAsync(cancellationToken);

            return builder.AddSqlite("sqlite", dbPath, "pulse.db");
        }

        public async Task<IResourceBuilder<IResourceWithConnectionString>> WithPostgres(
            CancellationToken cancellationToken = default)
        {
            var dbUsernameParameter = builder.AddParameter("DatabaseUsername");
            var dbPasswordParameter = builder.AddParameter("DatabasePassword");

            var postgres = builder
                .AddPostgres("postgres", dbUsernameParameter, dbPasswordParameter, 5432)
                .WithDataVolume()
                .WithLifetime(ContainerLifetime.Persistent);

            const string databaseName = "pulse";

            const string creationScript = $"""
                                           CREATE USER {databaseName} WITH PASSWORD {databaseName};
                                           CREATE DATABASE {databaseName};
                                           GRANT ALL PRIVILEGES ON DATABASE {databaseName} TO {databaseName};
                                           """;

            var postgresDatabase = postgres.AddDatabase(databaseName)
                .WithCreationScript(creationScript);

            return await Task.FromResult(postgresDatabase);
        }
    }
}