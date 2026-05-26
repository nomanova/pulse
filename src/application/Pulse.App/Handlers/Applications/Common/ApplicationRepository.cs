using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.App.Handlers.Applications.Common;

public interface IApplicationRepository : IRepository<Application>;

internal sealed class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(IDatabaseContext context) : base(context.Applications)
    {
    }
}