using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database.Specifications;
using Pulse.App.Common.Database.Specifications.Base;
using Throw;

namespace Pulse.App.Common.Database;

public class ReadRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet;

    protected ReadRepository(DbSet<TEntity> dbSet)
    {
        _dbSet = dbSet;
    }

    public async Task<List<TEntity>> Search(
        Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.WithSpecification(specification);
        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<TEntity?> SearchOne(
        Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.WithSpecification(specification);
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<uint> Count(
        Specification<TEntity> specification, CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.WithSpecification(specification);
        return (uint)await queryable.CountAsync(cancellationToken);
    }

    public async Task<TEntity?> SearchOne(
        Specification<TEntity> specification,
        IOrderBySpecification<TEntity> orderBy,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.WithSpecification(specification);
        queryable = orderBy.ApplyOrdering(queryable);
        return await queryable.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SearchResult<TEntity>> SearchCursor(
        Specification<TEntity> searchBy,
        IOrderBySpecification<TEntity> orderBy,
        uint pageSize,
        Specification<TEntity>? searchLast = null,
        CancellationToken cancellationToken = default)
    {
        var queryable = _dbSet.WithSpecification(searchBy);
        queryable = orderBy.ApplyOrdering(queryable);

        var pagedQueryable = queryable;

        if (searchLast != null)
        {
            var lastEntity = await SearchOne(searchLast, cancellationToken);
            lastEntity.ThrowIfNull();

            pagedQueryable = queryable.Where(orderBy.Comparable(lastEntity));
        }

        var entities = await pagedQueryable
            .Take((int)pageSize)
            .ToListAsync(cancellationToken);

        var hasNext = await HasNext(
            queryable, entities, orderBy, cancellationToken);

        return new SearchResult<TEntity>
        {
            HasNext = hasNext,
            Entities = entities
        };
    }

    private static async Task<bool> HasNext(
        IQueryable<TEntity> queryable,
        List<TEntity> entities,
        IOrderBySpecification<TEntity> orderBy,
        CancellationToken cancellationToken)
    {
        if (!entities.Any())
        {
            return false;
        }

        return await queryable
            .Where(orderBy.Comparable(entities))
            .AnyAsync(cancellationToken);
    }
}