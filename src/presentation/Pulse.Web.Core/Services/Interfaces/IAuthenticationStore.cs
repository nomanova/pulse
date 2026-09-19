using System.Threading.Tasks;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IAuthenticationStore
{
    Task SetToken(string token);
    
    Task<string?> GetToken();
    
    Task SetUser(UserProfile profile);
    
    Task<UserProfile?> GetUser();

    Task SetOrganization(OrganizationProfile profile);
    
    Task<OrganizationProfile?> GetOrganization();
    
    Task ClearUser();
    
    Task ClearAll();
}