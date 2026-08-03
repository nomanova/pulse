namespace Pulse.Domain.Aggregates.Applications;

public interface IApplicationScoped
{
    ApplicationId ApplicationId { get; }
}