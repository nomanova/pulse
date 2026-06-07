using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Common.Authorization;

public sealed class ResourceAuthorizationContextBehavior<TRequest, TResponse>(
    IResourceAuthorizationContextProvider provider) : 
    IPipelineBehavior<TRequest, TResponse>     
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IOrganizationScoped<string?> organizationScoped)
        {
            if (organizationScoped.OrganizationId == null)
            {
                throw new UnauthorizedException();
            }

            provider.Context = new ResourceAuthorizationContext
            {
                Scope = Scope.Organization,
                OrganizationId = OrganizationId.New(organizationScoped.OrganizationId)
            };
        }

        return await next();
    }
}