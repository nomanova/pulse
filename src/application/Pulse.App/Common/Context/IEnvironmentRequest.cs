namespace Pulse.App.Common.Context;

public interface IEnvironmentRequest : IApplicationRequest
{
    string? EnvironmentName { get; }
}