using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Organizations;
using Throw;

namespace Pulse.Tests.Shared.Builders;

public sealed class ApplicationBuilder : IBuilder<Application>
{
    private Organization? _organization;
    private string? _name;

    private ApplicationBuilder()
    {
    }

    public static ApplicationBuilder New(Organization organization)
    {
        return new ApplicationBuilder { _organization = organization };
    }

    public ApplicationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Application Build()
    {
        _organization.ThrowIfNull();
        _name.ThrowIfNull();

        return Application.Create(_name, _organization);
    }
}