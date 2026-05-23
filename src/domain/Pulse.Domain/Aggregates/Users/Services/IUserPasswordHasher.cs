namespace Pulse.Domain.Aggregates.Users.Services;

public interface IUserPasswordHasher
{
    string Hash(string text);

    bool Verify(string text, string hash);
}