using System.Collections.Generic;

namespace Pulse.App.Common.Database;

public sealed record SearchResult<TEntity>
{
    public bool HasNext { get; init; }
    
    public IReadOnlyList<TEntity> Entities { get; init; } = new List<TEntity>();
}