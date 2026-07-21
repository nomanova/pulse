using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Organizations;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;

namespace Pulse.App.Handlers.Organizations.Queries;

public sealed class FetchOrganizationQuery : IQuery<ErrorOr<OrganizationDto>>
{
    public required string? OrganizationName { get; init; }
}

public sealed class FetchOrganizationQueryAuthorizer : PermissionAuthorizer<FetchOrganizationQuery>;

public class FetchOrganizationQueryHandler : IQueryHandler<FetchOrganizationQuery, ErrorOr<OrganizationDto>>
{
    private readonly IOrganizationRepository _organizationRepository;

    public FetchOrganizationQueryHandler(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public async Task<ErrorOr<OrganizationDto>> Handle(FetchOrganizationQuery query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.OrganizationName))
        {
            return Error.NotFound();
        }

        // Fetch organization
        var specification = new OrganizationByNameSpecification(query.OrganizationName);
        var organization = await _organizationRepository.SearchOne(specification, cancellationToken);

        if (organization == null)
        {
            return Error.NotFound();
        }

        return organization.ToDto();
    }
}