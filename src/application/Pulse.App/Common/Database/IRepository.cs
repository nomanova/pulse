using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Database.Specifications.Base;

namespace Pulse.App.Common.Database;

public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
{
    void Add(TEntity entity);

    void AddRange(List<TEntity> entities);

    void Update(TEntity entity);

    void Remove(TEntity entity);

    Task Delete(Specification<TEntity> specification, CancellationToken cancellationToken = default);
}