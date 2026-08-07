using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Database.Specifications;

public sealed class OrderByIdSpecification<T, Tk> : 
    IOrderBySpecification<T> where T : Entity<Tk> where Tk : EntityId
{
    private readonly bool _ascending;
    
    public OrderByIdSpecification(bool? ascending = null)
    {
        _ascending = ascending ?? true;
    }

    public IQueryable<T> ApplyOrdering(IQueryable<T> queryable)
    {
        return _ascending
            ? queryable.OrderBy(entity => entity.Id)
            : queryable.OrderByDescending(entity => entity.Id);
    }

    public Expression<Func<T, bool>> Comparable(T entity)
    {
        return Comparable(entity.Id);
    }

    public Expression<Func<T, bool>> Comparable(List<T> entities)
    {
        var id = _ascending
            ? entities.Max(entity => entity.Id)
            : entities.Min(entity => entity.Id);

        return Comparable(id!);
    }

    private Expression<Func<T, bool>> Comparable(Tk id)
    {
        return _ascending
            ? item => item.Id.CompareTo(id) > 0
            : item => item.Id.CompareTo(id) < 0;
    }
}