using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Dispatcher;

namespace Pulse.App.Common.Authorization;

public sealed class AuthorizationBehavior<TRequest, TResponse>(
    IEnumerable<IAuthorizer<TRequest>> authorizers, ISender sender) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        var requirements = new List<IAuthorizationRequirement>();

        foreach (var authorizer in authorizers)
        {
            authorizer.BuildPolicy(request);
            requirements.AddRange(authorizer.Requirements);
        }
        
        foreach (var requirement in requirements.Distinct())
        {
            var result = await sender.Send(requirement, cancellationToken);

            if (result.IsError)
            {
                return (dynamic)result.Errors;
            }
        }

        return await next();
    }
}