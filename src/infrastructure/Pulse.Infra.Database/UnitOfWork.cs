using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Database;
using Pulse.Domain.Common.Models.Events;
using Pulse.Infra.Database.Contexts;
using Pulse.Infra.Database.Outbox;

namespace Pulse.Infra.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    private readonly IEventBus _eventBus;

    public UnitOfWork(DatabaseContext context, IEventBus eventBus)
    {
        _context = context;
        _eventBus = eventBus;
    }

    public async Task Commit(CancellationToken cancellationToken = default)
    {
        var domainEvents = _context.ChangeTracker
            .Entries<IDomainEventEmitter>()
            .Select(entry => entry.Entity)
            .SelectMany(eventEmitter =>
            {
                var domainEvents = eventEmitter.Events;
                eventEmitter.RemoveAllEvents();
                return domainEvents;
            })
            .Cast<object>()
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _eventBus.Publish(domainEvent, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}