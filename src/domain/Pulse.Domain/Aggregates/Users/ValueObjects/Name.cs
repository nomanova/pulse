using ErrorOr;
using System.Linq;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;
using Pulse.Domain.Common.Models.Text;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public record Name
{
    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    /**
     * Used for searching for users by name.
     * Normalized form does not contain diacritics and is lowercased.
     */
    public string NormalizedName { get; private set; } = null!;

    private Name()
    {
    }

    private Name(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
        NormalizedName = FullName.AsNormalizedQueryable();
    }

    internal static ErrorOr<Name> Create(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return DomainErrors.User.UsernameRequired;
        }

        var firstNameResult = firstName.AsName(nameof(FirstName));
        var lastNameResult = lastName.AsName(nameof(LastName));

        var errors = Errors.Map(firstNameResult, lastNameResult);

        if (errors.Any())
        {
            return errors;
        }

        return new Name(firstNameResult.Value, lastNameResult.Value);
    }
}