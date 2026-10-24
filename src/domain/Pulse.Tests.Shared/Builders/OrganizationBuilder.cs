using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.Tests.Shared.Builders;

public sealed class OrganizationBuilder : IBuilder<Organization>
{
    private string? _name;
    private string? _key;

    private OrganizationBuilder()
    {
    }

    public static OrganizationBuilder New()
    {
        return new OrganizationBuilder()
            .WithName("Princeton Plainsboro")
            .WithKey("princeton-plainsboro");
    }

    public OrganizationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public OrganizationBuilder WithKey(string key)
    {
        _key = key;
        return this;
    }

    public Organization Build()
    {
        return Organization.Create(_name, _key);
    }
}