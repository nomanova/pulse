using Pulse.App.Common.Database;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Organizations.Common;

public interface IOrganizationRepository : IRepository<Organization>;

internal sealed class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(IDatabaseContext context) : base(context.Organizations)
    {
    }
}