using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Errors;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Applications.Common.Specifications;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Environments.Common.Specifications;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Common.Context;

public sealed class ContextProviderBehavior<TRequest, TResponse> :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IErrorOr
{
    private readonly IContextProvider _contextProvider;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IApplicationRepository _applicationRepository;
    private readonly IEnvironmentRepository _environmentRepository;

    public ContextProviderBehavior(
        IContextProvider contextProvider,
        IOrganizationRepository organizationRepository,
        IApplicationRepository applicationRepository,
        IEnvironmentRepository environmentRepository)
    {
        _contextProvider = contextProvider;
        _organizationRepository = organizationRepository;
        _applicationRepository = applicationRepository;
        _environmentRepository = environmentRepository;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is IEnvironmentRequest environmentRequest)
        {
            var application = await ApplyApplicationContext(
                Scope.Environment,
                environmentRequest.OrganizationName,
                environmentRequest.ApplicationName,
                cancellationToken);

            if (application.IsError)
            {
                return ToResponse(application.Errors);
            }

            var environmentResult = await ResolveEnvironment(
                _contextProvider.Organization.Id,
                application.Value.Id,
                environmentRequest.EnvironmentName,
                cancellationToken);

            if (environmentResult.IsError)
            {
                return ToResponse(environmentResult.Errors);
            }

            _contextProvider.Environment = environmentResult.Value;
        }
        else if (request is IApplicationRequest applicationRequest)
        {
            var application = await ApplyApplicationContext(
                Scope.Application,
                applicationRequest.OrganizationName,
                applicationRequest.ApplicationName,
                cancellationToken);

            if (application.IsError)
            {
                return ToResponse(application.Errors);
            }
        }
        else if (request is IOrganizationRequest organizationRequest)
        {
            var organization = await ApplyOrganizationContext(
                Scope.Organization,
                organizationRequest.OrganizationName,
                cancellationToken);

            if (organization.IsError)
            {
                return ToResponse(organization.Errors);
            }
        }
        else
        {
            _contextProvider.Scope = Scope.Server;
        }

        return await next();
    }

    private async Task<ErrorOr<Organization>> ApplyOrganizationContext(
        Scope scope,
        string? organizationName,
        CancellationToken cancellationToken)
    {
        _contextProvider.Scope = scope;

        var organizationResult = await ResolveOrganization(organizationName, cancellationToken);

        if (organizationResult.IsError)
        {
            return organizationResult.Errors;
        }

        _contextProvider.Organization = organizationResult.Value;

        return organizationResult.Value;
    }

    private async Task<ErrorOr<Application>> ApplyApplicationContext(
        Scope scope,
        string? organizationName,
        string? applicationName,
        CancellationToken cancellationToken)
    {
        var organizationResult = await ApplyOrganizationContext(
            scope,
            organizationName,
            cancellationToken);

        if (organizationResult.IsError)
        {
            return organizationResult.Errors;
        }

        var applicationResult = await ResolveApplication(
            organizationResult.Value.Id,
            applicationName,
            cancellationToken);

        if (applicationResult.IsError)
        {
            return applicationResult.Errors;
        }

        _contextProvider.Application = applicationResult.Value;

        return applicationResult.Value;
    }

    private async Task<ErrorOr<Organization>> ResolveOrganization(
        string? organizationName, CancellationToken cancellationToken)
    {
        if (organizationName == null)
        {
            return ApplicationErrors.NotFound("Organization");
        }

        var specification = new OrganizationByNameSpecification(organizationName);
        var organization = await _organizationRepository.SearchOne(specification, cancellationToken);

        return organization == null ? ApplicationErrors.NotFound("Organization") : organization;
    }

    private async Task<ErrorOr<Application>> ResolveApplication(
        OrganizationId organizationId, string? applicationName, CancellationToken cancellationToken)
    {
        if (applicationName == null)
        {
            return ApplicationErrors.NotFound("Application");
        }

        var specification = new ApplicationByNameSpecification(organizationId, applicationName);
        var application = await _applicationRepository.SearchOne(specification, cancellationToken);

        return application == null ? ApplicationErrors.NotFound("Application") : application;
    }

    private async Task<ErrorOr<Environment>> ResolveEnvironment(
        OrganizationId organizationId, ApplicationId applicationId, string? environmentName,
        CancellationToken cancellationToken)
    {
        if (environmentName == null)
        {
            return ApplicationErrors.NotFound("Environment");
        }

        var specification = new EnvironmentByNameSpecification(organizationId, applicationId, environmentName);
        var environment = await _environmentRepository.SearchOne(specification, cancellationToken);

        return environment == null ? ApplicationErrors.NotFound("Environment") : environment;
    }

    private static TResponse ToResponse(List<Error> errors)
    {
        return (dynamic)errors;
    }
}