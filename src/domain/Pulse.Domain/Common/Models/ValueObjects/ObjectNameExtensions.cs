using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.ValueObjects;

public static class ObjectNameExtensions
{
    extension(string? name)
    {
        public ObjectName AsName()
        {
            return ObjectName.Create(name).Assert();
        }

        public string AsNormalizedObjectName()
        {
            return name is null ? "" : name.Normalize().Replace("-", "");
        }
    }
}