using System;
using System.Collections.Generic;
using System.Linq;
using Pulse.Domain.Common.Models.Events;

namespace Pulse.Domain.Common.Models.Entities;

/// <summary>
/// Base class for domain entities.
/// </summary>
/// <remarks>
/// This class adds capabilities which are used by practically each entity in the domain.
/// In particular, the ability to record and emit events as part of processing state changes.
/// </remarks>
public abstract class DomainEntity<T> : Entity<T>, IDomainEntity where T : EntityId
{
    protected DomainEntity()
    {
    }

    protected DomainEntity(T id) : base(id)
    {
    }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ModifiedAt { get; private set; }

    public DateTime? DeletedAt { get; private set; }

    public bool IsDeleted { get; private set; }

    public uint Version { get; set; }
    
    protected void SetCreated()
    {
        CreatedAt = DateTime.UtcNow;
    }

    protected void SetModified()
    {
        ModifiedAt = DateTime.UtcNow;
    }

    public virtual void SetDeleted()
    {
        if (IsDeleted)
        {
            return;
        }

        DeletedAt = DateTime.UtcNow;
        IsDeleted = true;
    }

    private readonly List<IDomainEvent> _events = [];

    public IReadOnlyList<IDomainEvent> Events => _events.ToList();

    public void AddEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public void RemoveEvent(IDomainEvent domainEvent)
    {
        _events.Remove(domainEvent);
    }

    public void RemoveAllEvents()
    {
        _events.Clear();
    }
}