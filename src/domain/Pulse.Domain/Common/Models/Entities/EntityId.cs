using System;

namespace Pulse.Domain.Common.Models.Entities;

public abstract record EntityId : IComparable<EntityId>
{
    public required string Value { get; init; }

    public sealed override string ToString()
    {
        return Value;
    }
    
    public int CompareTo(EntityId? other)
    {
        return other is null ? 1 : 
            string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

    public static bool operator >(EntityId? left, EntityId? right)
    {
        return left != null && left.CompareTo(right) > 0;
    }
    
    public static bool operator <(EntityId? left, EntityId? right)
    {
        return right != null && right.CompareTo(left) > 0;
    }
}