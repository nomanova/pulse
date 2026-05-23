using BCrypt.Net;
using Pulse.Domain.Aggregates.Users.Services;
using Throw;

namespace Pulse.Infra.Security.Password;

public sealed class BCryptPasswordHasher : IUserPasswordHasher
{
    public string Hash(string text)
    {
        text.ThrowIfNull();
        
        return BCrypt.Net.BCrypt.EnhancedHashPassword(text, HashType.SHA384);
    }
    
    public bool Verify(string text, string hash)
    {
        text.ThrowIfNull();
        hash.ThrowIfNull();
        
        // ReSharper disable once RedundantArgumentDefaultValue
        return BCrypt.Net.BCrypt.EnhancedVerify(text, hash, HashType.SHA384);
    }
}