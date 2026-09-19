using System.Threading.Tasks;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IAuthenticationService
{
    Task<bool> SignIn(string? username, string? password);
    
    Task<bool> SignOut();

    Task<OrganizationProfile?> GetOrganization();
    
    Task SwitchOrganization(OrganizationProfile profile);
}