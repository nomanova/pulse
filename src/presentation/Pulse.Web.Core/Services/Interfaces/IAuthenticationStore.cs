using System.Threading.Tasks;
using Pulse.App.Dto.Users;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IAuthenticationStore
{
    Task Set(AuthDto auth);

    Task<AuthDto?> Get();
    
    Task Clear();
}