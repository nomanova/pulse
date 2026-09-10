using System.Threading.Tasks;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IAuthenticationService
{
    Task<UserProfile?> UserProfile();
    
    Task<bool> SignIn(string? username, string? password);
    
    Task<bool> SignOut();
}