using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Database;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Domain.Common.Models.Events;
using Pulse.Infra.Database.Contexts;
using Pulse.Infra.Database.Messaging.Outbox;

namespace Pulse.Infra.Database;

public class UnitOfWork : IUnitOfWork
{
    private readonly DatabaseContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UnitOfWork(DatabaseContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
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
            _context.InsertOutboxMessage(_dateTimeProvider, domainEvent);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}