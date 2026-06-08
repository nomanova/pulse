using Pulse.Domain.Aggregates.Users.Services;

namespace Pulse.Domain.Tests.Shared.Fakers;

public sealed class FakePasswordHasher : IUserPasswordHasher
{
    public static FakePasswordHasher Default => new();
    
    public string Hash(string text) => "hashed_" + text;

    public bool Verify(string text, string hash) => hash == "hashed_" + text;
}