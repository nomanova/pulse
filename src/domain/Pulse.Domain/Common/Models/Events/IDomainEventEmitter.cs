using System.Collections.Generic;

namespace Pulse.Domain.Common.Models.Events;

public interface IDomainEventEmitter
{
    void AddEvent(IDomainEvent domainEvent);

    void RemoveEvent(IDomainEvent domainEvent);

    void RemoveAllEvents();

    IReadOnlyList<IDomainEvent> Events { get; }
}