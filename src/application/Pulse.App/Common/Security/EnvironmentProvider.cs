using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.Domain.Aggregates.Environments;

namespace Pulse.App.Common.Security;

public class EnvironmentProvider : IEnvironmentProvider
{
    private readonly IUserClaimProvider _userClaimProvider;
    private readonly IEnvironmentRepository _environmentRepository;
    
    private Environment? _environment;

    public EnvironmentProvider(
        IUserClaimProvider userClaimProvider, 
        IEnvironmentRepository environmentRepository)
    {
        _userClaimProvider = userClaimProvider;
        _environmentRepository = environmentRepository;
    }

    public async Task<Environment> Get(CancellationToken cancellationToken = default)
    {
        if (_environment != null)
        {
            return _environment;
        }
        
        var apiKey = _userClaimProvider.ApiKey;
        var specification = new EnvironmentByApiKeySpecification(apiKey);

        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        if (environment == null)
        {
            throw new UnauthorizedException();
        }

        _environment = environment;

        return _environment;
    }
}