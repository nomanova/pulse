using System.Collections.Generic;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IContextProvider
{
    NamedIdentity? Organization { get; set; }

    Task<ErrorOr<Success>> EnsureOrganization(string organizationId);
    
    NamedIdentity? Application { get; }

    Task<ErrorOr<Success>> EnsureApplication(string applicationId, string? environmentId = null);
    
    IReadOnlyList<NamedIdentity>? Environments { get; }
    
    NamedIdentity? CurrentEnvironment { get; set; }
}