using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Pulse.Database.Contexts;

public sealed class PostgresDatabaseContext : DatabaseContext
{
    public PostgresDatabaseContext(
        IOptions<DatabaseOptions> databaseOptions,
        DbContextOptions contextOptions) : base(databaseOptions, contextOptions)
    {
    }
}