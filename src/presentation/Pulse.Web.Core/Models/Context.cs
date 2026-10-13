using System.Collections.Generic;
using Pulse.App.Dto.Environments;

namespace Pulse.Web.Core.Models;

public sealed record Context
{
    public required string OrganizationId { get; init; }

    public required string ApplicationId { get; init; }

    public required string ApplicationName { get; init; }

    public required IReadOnlyList<EnvironmentDto> Environments { get; init; }

    public required EnvironmentDto CurrentEnvironment { get; init; }
}

public static class ContextExtensions
{
    public static bool IsLoaded(this Context? context, string organizationId, string applicationId,
        string? environmentId)
    {
        if (environmentId == null)
        {
            return false;
        }

        return context != null &&
               context.OrganizationId == organizationId &&
               context.ApplicationId == applicationId &&
               context.CurrentEnvironment.Id == environmentId;
    }
}