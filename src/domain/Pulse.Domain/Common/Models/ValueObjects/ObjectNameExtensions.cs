using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.ValueObjects;

public static class ObjectNameExtensions
{
    public static ObjectName AsName(this string? name)
    {
        return ObjectName.Create(name).Assert();
    }
}