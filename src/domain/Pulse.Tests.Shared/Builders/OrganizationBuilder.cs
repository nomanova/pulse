using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.Tests.Shared.Builders;

public sealed class OrganizationBuilder : IBuilder<Organization>
{
    private string? _name;

    private OrganizationBuilder()
    {
    }

    public static OrganizationBuilder New()
    {
        return new OrganizationBuilder()
            .WithName("princeton-plainsboro");
    }

    public OrganizationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public Organization Build()
    {
        return Organization.Create(_name);
    }
}