using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database.Specifications.Base;

namespace Pulse.App.Common.Database;

public static class DbSetExtensions
{
    public static IQueryable<TEntity> WithSpecification<TEntity>(
        this DbSet<TEntity> dbSet, Specification<TEntity> specification) where TEntity : class
    {
        var queryable = dbSet.Where(specification.ToExpression());

        if (!specification.WithIncludes)
        {
            queryable = queryable.IgnoreAutoIncludes();
        }
        
        return queryable;
    }
}