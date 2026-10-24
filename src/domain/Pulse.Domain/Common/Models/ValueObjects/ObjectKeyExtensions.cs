namespace Pulse.Domain.Common.Models.ValueObjects;

public static class ObjectKeyExtensions
{
    extension(string? key)
    {
        public string AsNormalizedObjectKey()
        {
            return key is null ? "" : key.Normalize().Replace("-", "");
        }
    }
}