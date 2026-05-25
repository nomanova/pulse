using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pulse.App.Common.Database.Specifications;

public interface IOrderBySpecification<T>
{
    IQueryable<T> ApplyOrdering(IQueryable<T> queryable);

    Expression<Func<T, bool>> Comparable(T entity);

    Expression<Func<T, bool>> Comparable(List<T> entities);
}