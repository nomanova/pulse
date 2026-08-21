using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Pulse.Infra.Database.ValueComparers;

public sealed class DictionaryValueComparer<TKey, TValue> : ValueComparer<Dictionary<TKey, TValue>>
    where TKey : notnull
{
    public DictionaryValueComparer()
        : base(
            (left, right) => AreEqual(left, right),
            dictionary => GetHashCode(dictionary),
            dictionary => Snapshot(dictionary))
    {
    }

    private static bool AreEqual(Dictionary<TKey, TValue>? left, Dictionary<TKey, TValue>? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null || left.Count != right.Count)
        {
            return false;
        }

        var valueComparer = EqualityComparer<TValue>.Default;

        foreach (var leftItem in left)
        {
            if (!right.TryGetValue(leftItem.Key, out var rightValue))
            {
                return false;
            }

            if (!valueComparer.Equals(leftItem.Value, rightValue))
            {
                return false;
            }
        }

        return true;
    }

    private new static int GetHashCode(Dictionary<TKey, TValue> dictionary)
    {
        var hash = new HashCode();

        foreach (var item in dictionary.OrderBy(item => item.Key))
        {
            hash.Add(item.Key);
            hash.Add(item.Value);
        }

        return hash.ToHashCode();
    }

    private new static Dictionary<TKey, TValue> Snapshot(Dictionary<TKey, TValue> dictionary)
    {
        return dictionary.ToDictionary(item => item.Key, item => item.Value);
    }
}