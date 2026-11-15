using System.Collections.Generic;
using System.Threading.Tasks;
using ErrorOr;
using Pulse.Web.Core.Models;

namespace Pulse.Web.Core.Services.Interfaces;

public interface IContextProvider
{
    ObjectIdentity? Organization { get; set; }

    Task<ErrorOr<Success>> EnsureOrganization(string organizationId);
    
    ObjectIdentity? Application { get; }

    Task<ErrorOr<Success>> EnsureApplication(string applicationId, string? environmentId = null);

    void AddAndSwitchToEnvironment(string applicationId, ObjectIdentity environment);
    
    void SwitchEnvironment(string applicationId, string environmentId);

    void RemoveEnvironment(string applicationId, string environmentId);

    IReadOnlyList<ObjectIdentity>? Environments { get; }
    
    ObjectIdentity? CurrentEnvironment { get; set; }
}