using System.Threading.Tasks;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IAuthenticationService
{
    Task<bool> SignIn(string? username, string? password);
    
    Task<bool> SignOut();
}