using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using ErrorOr;
using Pulse.Api.Client.Common;
using Pulse.Api.Ctrl.Client;
using Pulse.Api.Ctrl.Contract;
using Pulse.Web.Core.Models;
using Pulse.Web.Core.Services.Interfaces;

namespace Pulse.Web.Core.Services;

public sealed record ContextChangedMessage;

public sealed class ContextProvider : IContextProvider
{
    private const string ErrLoadOrganization = "Loading organization failed";
    private const string ErrLoadApplication = "Loading application failed";
    private const string ErrLoadEnvironments = "Loading environments failed";
    private const string ErrEnvironmentNotFound = "Environment not found";

    private readonly IMessenger _messenger;
    private readonly ICtrlApiClient _apiClient;

    public ContextProvider(
        IMessenger messenger,
        ICtrlApiClient apiClient)
    {
        _messenger = messenger;
        _apiClient = apiClient;
    }

    public NamedIdentity? Organization
    {
        get;
        set
        {
            Application = null;
            Environments = null;
            CurrentEnvironment = null;
            field = value;

            _messenger.Send(new ContextChangedMessage());
        }
    }

    public async Task<ErrorOr<Success>> EnsureOrganization(string organizationId)
    {
        if (Organization != null && Organization.Id == organizationId)
        {
            return Result.Success;
        }

        var result = await _apiClient.Organizations.Fetch(new FetchOrganizationRequest
        {
            OrganizationId = organizationId
        });

        if (!result.IsSuccessWithData())
        {
            return Error.Failure(description: ErrLoadOrganization);
        }

        var organization = result.Data!;
        Organization = new NamedIdentity(organization.Id, organization.Name);

        return Result.Success;
    }

    public NamedIdentity? Application { get; private set; }

    public async Task<ErrorOr<Success>> EnsureApplication(string applicationId, string? environmentId = null)
    {
        if (Application != null && Application.Id == applicationId)
        {
            return Result.Success;
        }

        // Load application
        var applicationResult = await _apiClient.Applications.Fetch(new FetchApplicationRequest
        {
            ApplicationId = applicationId
        });

        if (!applicationResult.IsSuccessWithData())
        {
            return Error.Failure(description: ErrLoadApplication);
        }

        var application = applicationResult.Data!;

        // Load environments
        var environmentsResult = await _apiClient.Environments.Search(new SearchEnvironmentsRequest
        {
            ApplicationId = applicationId,
            PageSize = Constants.MaxEnvironmentCount
        });

        if (!environmentsResult.IsSuccessWithData())
        {
            return Error.Failure(description: ErrLoadEnvironments);
        }

        var environments = environmentsResult.Data!.Entities;

        // Set
        var currentEnvironment = environmentId == null
            ? environments.FirstOrDefault(env => env.IsPrimary)
            : environments.FirstOrDefault(env => env.Id == environmentId);

        if (currentEnvironment == null)
        {
            return Error.NotFound(description: ErrEnvironmentNotFound);
        }

        Application = new NamedIdentity(application.Id, application.Name);
        Environments = environments.Select(env => new NamedIdentity(env.Id, env.Name)).ToList().AsReadOnly();
        CurrentEnvironment = new NamedIdentity(currentEnvironment.Id, currentEnvironment.Name);

        return Result.Success;
    }

    public IReadOnlyList<NamedIdentity>? Environments { get; private set; }

    public NamedIdentity? CurrentEnvironment
    {
        get;
        set
        {
            if (value != null && !(Environments ?? []).Select(env => env.Id).Contains(value.Id))
            {
                throw new InvalidOperationException("Unknown environment");
            }

            field = value;
        }
    }
}