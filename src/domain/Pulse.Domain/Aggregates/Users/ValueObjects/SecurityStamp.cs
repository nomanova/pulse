using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record SecurityStamp
{
    public string Value { get; private set; } = null!;

    private SecurityStamp()
    {
    }

    private SecurityStamp(string value)
    {
        Value = value;
    }

    public static SecurityStamp Create()
    {
        return new SecurityStamp(Generate());
    }

    public static SecurityStamp FromValue(string value)
    {
        return new SecurityStamp(value);
    }

    public void Reset()
    {
        Value = Generate();
    }

    private static string Generate()
    {
        return IdentityProvider.New();
    }
}