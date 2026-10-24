using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Database.Specifications;

public static class Extensions
{
    public static Expression<Func<T, bool>> WithKeyFilter<T>(
        this Expression<Func<T, bool>> expr, string? query) where T : IKeyedObject
    {
        if (!string.IsNullOrEmpty(query))
        {
            Expression<Func<T, bool>> matchesQueryExpr = entity =>
                EF.Functions.Like(entity.Key.NormalizedValue, $"%{query}%");
            expr = expr.AndAlso(matchesQueryExpr);
        }

        return expr;
    }

    public static Expression<Func<T, bool>> WithKeyOrNameFilter<T>(
        this Expression<Func<T, bool>> expr, string? query) where T : IKeyedObject, INamedObject
    {
        if (!string.IsNullOrEmpty(query))
        {
            Expression<Func<T, bool>> matchesQueryExpr = entity =>
                EF.Functions.Like(entity.Key.NormalizedValue, $"%{query}%") ||
                EF.Functions.Like(entity.Name.NormalizedValue, $"%{query}%");
            expr = expr.AndAlso(matchesQueryExpr);
        }

        return expr;
    }
}