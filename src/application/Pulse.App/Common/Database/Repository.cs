using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database.Specifications.Base;

namespace Pulse.App.Common.Database;

public class Repository<TEntity> :
    ReadRepository<TEntity>, IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;

    protected Repository(DbSet<TEntity> dbSet) : base(dbSet)
    {
        _dbSet = dbSet;
    }

    public void Add(TEntity entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(List<TEntity> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task Delete(Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        await _dbSet.WithSpecification(specification).ExecuteDeleteAsync(cancellationToken);
    }
}