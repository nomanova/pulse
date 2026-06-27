using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pulse.Database;

public class MigrateDatabaseHostedService<TDbContext> : IHostedService
    where TDbContext : DbContext
{
    private readonly ILogger<MigrateDatabaseHostedService<TDbContext>> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    
    public MigrateDatabaseHostedService(
        ILogger<MigrateDatabaseHostedService<TDbContext>> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Applying migrations for {DbContext}", typeof(TDbContext).Name);

        var scope = _scopeFactory.CreateScope();

        try
        {
            var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
            await context.Database.MigrateAsync(cancellationToken);

            _logger.LogInformation("Migrations completed for {DbContext}", typeof(TDbContext).Name);
        }
        finally
        {
            if (scope is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                scope.Dispose();
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}