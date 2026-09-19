using Pulse.App.Dto.Organizations;

namespace Pulse.Web.Core.Models;

public sealed record OrganizationProfile
{
    public required string Id { get; init; }

    public required string Name { get; init; }
}

public static class OrganizationProfileExtensions
{
    public static OrganizationProfile ToOrganizationProfile(this OrganizationDto organization)
    {
        return new OrganizationProfile
        {
            Id = organization.Id,
            Name = organization.Name
        };
    }
}