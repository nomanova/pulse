using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.Users.Common.Specifications;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Common.Security;

public sealed class UserProvider : IUserProvider
{
    private readonly IUserClaimProvider _userClaimProvider;
    private readonly IUserRepository _userRepository;

    private User? _user;

    public UserProvider(
        IUserClaimProvider userClaimProvider,
        IUserRepository userRepository)
    {
        _userClaimProvider = userClaimProvider;
        _userRepository = userRepository;
    }

    public async Task<User> Get(CancellationToken cancellationToken = default)
    {
        if (_user != null)
        {
            return _user;
        }
        
        var userId = _userClaimProvider.Id;
        var user = await _userRepository.SearchOne(new UserByIdSpecification(UserId.New(userId)), cancellationToken);
        
        if (user == null)
        {
            throw new UnauthorizedException();
        }

        _user = user;

        return _user;
    }
}