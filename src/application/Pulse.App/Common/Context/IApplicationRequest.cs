namespace Pulse.App.Common.Context;

public interface IApplicationRequest : IOrganizationRequest
{
    string? ApplicationName { get; }
}