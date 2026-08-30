using System.Threading;
using System.Threading.Tasks;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Common.Security.Interfaces;

public interface IEnvironmentProvider
{
    Task<Environment> Get(CancellationToken cancellationToken = default);
}