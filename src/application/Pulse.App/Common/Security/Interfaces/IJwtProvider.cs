using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Common.Security.Interfaces;

public interface IJwtProvider
{
    string Create(User user);
}