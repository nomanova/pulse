using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Exceptions;
using Pulse.App.Common.Security.Interfaces;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
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
        if (request is IEnvironmentScoped environmentScoped)
        {
            if (environmentScoped.OrganizationId == null ||
                environmentScoped.ApplicationId == null ||
                environmentScoped.EnvironmentId == null)
            {
                throw new UnauthorizedException();
            }

            provider.Context = new ResourceAuthorizationContext
            {
                Scope = Scope.Environment,
                OrganizationId = environmentScoped.OrganizationId,
                ApplicationId = environmentScoped.ApplicationId,
                EnvironmentId = environmentScoped.EnvironmentId
            };
        }
        else if (request is IApplicationScoped applicationScoped)
        {
            if (applicationScoped.OrganizationId == null ||
                applicationScoped.ApplicationId == null)
            {
                throw new UnauthorizedException();
            }

            provider.Context = new ResourceAuthorizationContext
            {
                Scope = Scope.Application,
                OrganizationId = applicationScoped.OrganizationId,
                ApplicationId = applicationScoped.ApplicationId
            };
        }
        else if (request is IOrganizationScoped organizationScoped)
        {
            if (organizationScoped.OrganizationId == null)
            {
                throw new UnauthorizedException();
            }

            provider.Context = new ResourceAuthorizationContext
            {
                Scope = Scope.Organization,
                OrganizationId = organizationScoped.OrganizationId
            };
        }

        return await next();
    }
}