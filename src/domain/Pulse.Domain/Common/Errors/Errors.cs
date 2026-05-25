using System.Collections.Generic;
using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static class Errors
{
    public static List<Error> Map(params IErrorOr[]? items)
    {
        var errors = new List<Error>();

        if (items == null || items.Length == 0)
        {
            return [];
        }

        foreach (var item in items)
        {
            if (!item.IsError || item.Errors == null || item.Errors.Count == 0)
            {
                continue;
            }

            errors.AddRange(item.Errors);
        }

        return errors;
    }
}