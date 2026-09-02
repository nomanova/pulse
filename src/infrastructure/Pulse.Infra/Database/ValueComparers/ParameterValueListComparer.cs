using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Pulse.Domain.Channels;

namespace Pulse.Infra.Database.ValueComparers;

public sealed class ParameterValueListComparer : ValueComparer<IReadOnlyList<ParameterValue>>
{
    public ParameterValueListComparer()
        : base(
            (left, right) => left!.SequenceEqual(right!),
            value => value.Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
            value => value.ToList())
    {
    }
}