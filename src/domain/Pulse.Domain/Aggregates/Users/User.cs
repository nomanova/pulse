using ErrorOr;
using Pulse.Domain.Aggregates.Users.ValueObjects;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Entities;
using Pulse.Domain.Common.Models.Text;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Users;

public class User : DomainEntity<UserId>, INamed
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string NormalizedName { get; private set; } = null!;

    public SecurityStamp SecurityStamp { get; private set; } = null!;
    
    private User()
    {
    }

    private User(
        UserId id,
        string firstName,
        string lastName,
        string name,
        string normalizedName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Name = name;
        NormalizedName = normalizedName;
    }

    public static ErrorOr<User> Create(string? firstName, string? lastName)
    {
        var firstNameValue = firstName.AsName(nameof(FirstName)).Assert();
        var lastNameValue = lastName.AsName(nameof(LastName)).Assert();

        var name = $"{firstNameValue} {lastNameValue}";
        var normalizedName = name.AsNormalizedQueryable();

        var id = IdentityProvider.New<UserId>();

        var user = new User(id, firstNameValue, lastNameValue, name, normalizedName);

        user.SetCreated();

        return user;
    }
    
    public override string ToString()
    {
        return $"[{Id.Value}] {Name}";
    }
}