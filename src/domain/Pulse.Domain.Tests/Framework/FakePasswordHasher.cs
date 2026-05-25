using Pulse.Domain.Aggregates.Users.Services;

namespace Pulse.Domain.Tests.Framework;

public sealed class FakePasswordHasher : IUserPasswordHasher
{
    public string Hash(string text) => "hashed_" + text;

    public bool Verify(string text, string hash) => hash == "hashed_" + text;
}