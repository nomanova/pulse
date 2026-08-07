using System.Security.Cryptography;

namespace Pulse.Domain.Aggregates.Environments.ValueObjects;

public sealed record ApiKey
{
    private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int Length = 52;

    public string Primary { get; private set; } = null!;

    public string Secondary { get; private set; } = null!;

    private ApiKey()
    {
    }

    private ApiKey(string primary, string secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }

    public static ApiKey Create()
    {
        return new ApiKey(GenerateSecret(), GenerateSecret());
    }

    public void RotatePrimary()
    {
        Primary = GenerateSecret();
    }

    public void RotateSecondary()
    {
        Secondary = GenerateSecret();
    }

    public bool IsValid(string key)
    {
        return key == Primary || key == Secondary;
    }

    private static string GenerateSecret()
    {
        return RandomNumberGenerator.GetString(AllowedCharacters, Length);
    }
}