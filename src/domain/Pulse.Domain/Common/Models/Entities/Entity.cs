using System;

namespace Pulse.Domain.Common.Models.Entities;

/// <summary>
/// Base class for entities.
/// </summary>
public abstract class Entity<T> : IEquatable<Entity<T>> where T : EntityId
{
    public T Id { get; }

    protected Entity(T id) => Id = id;

    // Reserved for ORM/serializer hydration; ID is populated via reflection immediately after construction.
    protected Entity() => Id = null!;

    public bool Equals(Entity<T>? other)
    {
        if (other is null)
        {
            return false;
        }
        
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        
        return GetType() == other.GetType() && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Entity<T>);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity<T>? left, Entity<T>? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Entity<T>? left, Entity<T>? right) => !(left == right);
}