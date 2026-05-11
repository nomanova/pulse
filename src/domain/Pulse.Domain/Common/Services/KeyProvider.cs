using System.Security.Cryptography;

namespace Pulse.Domain.Common.Services;

/**
 * Provides URL-safe random keys.
 */
public static class KeyProvider
{
    private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const uint KeyLength = 42;

    public static string New()
    {
        var key = new char[KeyLength];

        for (var i = 0; i < key.Length; i++)
        {
            var nextPosition = RandomNumberGenerator.GetInt32(0, Alphabet.Length);
            var nextChar = Alphabet[nextPosition];

            key[i] = nextChar;
        }

        return new string(key);
    }
}