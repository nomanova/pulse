using Pulse.Api.Ctrl.Client.Users;

namespace Pulse.Api.Ctrl.Client;

public interface ICtrlApiClient
{
    IUsersService Users { get; }
}