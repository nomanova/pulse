using ErrorOr;
using FluentValidation;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Domain.Common.Errors;
using Throw;

namespace Pulse.Domain.Aggregates.Users.ValueObjects;

public sealed record Password
{
    public const int MinLength = 8;
    public const int MaxLength = 50;

    private string? Value { get; }

    public string? HashedValue { get; private set; }

    private Password()
    {
    }

    private Password(string value)
    {
        Value = value;
    }

    internal static ErrorOr<Password> Create(string? value, IUserPasswordHasher passwordHasher)
    {
        passwordHasher.ThrowIfNull();

        var password = new Password(value!);

        var validator = new PasswordValidator();
        var result = validator.Validate(password);

        if (result.Errors.Count != 0)
        {
            return result.ToErrorOr<Password>();
        }

        password.HashedValue = passwordHasher.Hash(value!);
        return password;
    }

    public static Password FromHash(string? hashedValue)
    {
        return new Password
        {
            HashedValue = hashedValue
        };
    }
    
    private sealed class PasswordValidator : AbstractValidator<Password>
    {
        public PasswordValidator()
        {
            RuleFor(p => p.Value).NotNull().NotEmpty().WithError(BusinessErrors.User.PasswordRequired);
            RuleFor(p => p.Value).MinimumLength(MinLength).WithError(BusinessErrors.User.PasswordTooShort);
            RuleFor(p => p.Value).MaximumLength(MaxLength).WithError(BusinessErrors.User.PasswordTooLong);
        }
    }
}