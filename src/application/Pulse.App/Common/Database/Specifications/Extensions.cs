using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Database.Specifications.Base;
using Pulse.Domain.Common.Models.Entities;

namespace Pulse.App.Common.Database.Specifications;

public static class Extensions
{
    public static Expression<Func<T, bool>> WithNameFilter<T>(
        this Expression<Func<T, bool>> expr, string? query) where T : INamedObject
    {
        if (!string.IsNullOrEmpty(query))
        {
            Expression<Func<T, bool>> matchesQueryExpr = entity =>
                EF.Functions.Like(entity.Name.NormalizedValue, $"%{query}%");
            expr = expr.AndAlso(matchesQueryExpr);
        }

        return expr;
    }
}