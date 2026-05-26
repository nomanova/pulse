using System;

namespace Pulse.Domain.Common.Models.Entities;

public abstract record EntityId : IComparable<EntityId>, IComparable
{
    public string Value { get; protected init; } = null!;

    public sealed override string ToString()
    {
        return Value;
    }

    public int CompareTo(EntityId? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null ? 1 : string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is EntityId other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(EntityId)}");
    }

    public static bool operator >(EntityId? left, EntityId? right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator <(EntityId? left, EntityId? right)
    {
        return right is not null && right.CompareTo(left) > 0;
    }

    public static bool operator >=(EntityId? left, EntityId? right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    public static bool operator <=(EntityId? left, EntityId? right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }
}

/// <summary>
/// Base class for strongly typed entity identifiers.
/// </summary>
/// <typeparam name="TSelf">The type of the identifier itself (Curiously Recurring Template Pattern).</typeparam>
/// <typeparam name="TEntity">The type of the entity this identifier belongs to.</typeparam>
public abstract record EntityId<TSelf, TEntity> : EntityId, INew<TSelf>
    where TSelf : EntityId<TSelf, TEntity>, new()
{
    public static TSelf New(string value) => new() { Value = value };
}