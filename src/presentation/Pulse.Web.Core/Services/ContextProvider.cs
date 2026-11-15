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

public sealed record OrganizationChangedMessage;

public sealed record ApplicationChangedMessage;

public sealed record EnvironmentChangedMessage;

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

    public ObjectIdentity? Organization
    {
        get;
        set
        {
            Application = null;
            Environments = null;
            CurrentEnvironment = null;
            field = value;

            _messenger.Send(new OrganizationChangedMessage());
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
        Organization = new ObjectIdentity(organization.Id, organization.Name, organization.Key);

        return Result.Success;
    }

    public ObjectIdentity? Application { get; private set; }

    public async Task<ErrorOr<Success>> EnsureApplication(string applicationId, string? environmentId = null)
    {
        if (Application != null && Application.Id == applicationId)
        {
            if (environmentId == null)
            {
                return Result.Success;
            }

            // The application is already loaded, but the requested environment can differ from the current one
            // (e.g. browser back navigation after adding an environment), so sync it from the cached list.
            var cachedEnvironment = Environments?.FirstOrDefault(env => env.Id == environmentId);

            if (cachedEnvironment != null)
            {
                CurrentEnvironment = cachedEnvironment;
                return Result.Success;
            }

            // Unknown environment: fall through and reload from the API.
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

        Application = new ObjectIdentity(application.Id, application.Name, application.Key);
        Environments = environments.Select(env => env.ToIdentity()).ToList().AsReadOnly();
        CurrentEnvironment = currentEnvironment.ToIdentity();

        return Result.Success;
    }

    public void AddAndSwitchToEnvironment(string applicationId, ObjectIdentity environment)
    {
        if (Application == null || Application.Id != applicationId)
        {
            throw new InvalidOperationException("Faulty application");
        }
        
        if (Environments == null)
        {
            Environments = [environment];
        }
        else
        {
            Environments = [..Environments, environment];
        }

        CurrentEnvironment = environment;
        _messenger.Send(new EnvironmentChangedMessage());
    }

    public void SwitchEnvironment(string applicationId, string environmentId)
    {
        if (Application == null || Application.Id != applicationId)
        {
            throw new InvalidOperationException("Faulty application");
        }

        var environment = Environments?.FirstOrDefault(env => env.Id == environmentId)
                          ?? throw new InvalidOperationException("Unknown environment");

        CurrentEnvironment = environment;
        _messenger.Send(new EnvironmentChangedMessage());
    }

    public void RemoveEnvironment(string applicationId, string environmentId)
    {
        if (Application == null || Application.Id != applicationId)
        {
            throw new InvalidOperationException("Faulty application");
        }

        var remaining = (Environments ?? []).Where(env => env.Id != environmentId).ToList();
        var next = remaining.FirstOrDefault(env => env.IsPrimary) ?? remaining.FirstOrDefault();

        Environments = remaining.AsReadOnly();
        CurrentEnvironment = next;
        _messenger.Send(new EnvironmentChangedMessage());
    }

    public IReadOnlyList<ObjectIdentity>? Environments { get; private set; }

    public ObjectIdentity? CurrentEnvironment
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