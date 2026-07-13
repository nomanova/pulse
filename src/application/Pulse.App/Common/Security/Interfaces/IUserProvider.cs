using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Common.Security.Interfaces;

public interface IUserProvider
{
    Task<User> Get(CancellationToken cancellationToken = default);
}