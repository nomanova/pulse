using System;
using Microsoft.EntityFrameworkCore;

namespace Pulse.Infra.Database.Contexts;

public static class DbContextOptionsBuilderExtensions
{
    private const string DefaultSchema = "public";
    private const string MigrationsTable = "migrations";

    public static void Configure(this DbContextOptionsBuilder builder, DatabaseOptions options)
    {
        switch (options.Provider)
        {
            case DatabaseProvider.Postgres:
                builder.UseNpgsql(options.ConnectionString,
                    db => { db.MigrationsHistoryTable(MigrationsTable, options.Schema ?? DefaultSchema); });
                break;
            case DatabaseProvider.Sqlite:
                builder.UseSqlite(options.ConnectionString,
                    db => { db.MigrationsHistoryTable(MigrationsTable, options.Schema ?? DefaultSchema); });
                break;
            default:
                throw new NotImplementedException();
        }
    }
}