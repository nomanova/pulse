using System.Threading;
using System.Threading.Tasks;

namespace Pulse.App.Common.Database;

public interface IUnitOfWork
{
    Task Commit(CancellationToken cancellationToken = default);
}