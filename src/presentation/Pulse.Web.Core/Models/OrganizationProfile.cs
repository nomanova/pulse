using Pulse.App.Dto.Organizations;

namespace Pulse.Web.Core.Models;

public sealed record OrganizationProfile(string Id, string Name, string Key) : ObjectIdentity(Id, Name, Key);

public static class OrganizationProfileExtensions
{
    public static OrganizationProfile ToOrganizationProfile(this OrganizationDto organization)
    {
        return new OrganizationProfile(organization.Id, organization.Name, organization.Key);
    }
}