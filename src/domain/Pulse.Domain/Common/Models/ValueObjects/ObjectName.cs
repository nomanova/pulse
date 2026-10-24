using ErrorOr;
using FluentValidation;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Extensions;

namespace Pulse.Domain.Common.Models.ValueObjects;

public sealed record ObjectName
{
    public const int MaxLength = 100;

    public string Value { get; } = null!;

    public string NormalizedValue { get; } = null!;

    private ObjectName()
    {
    }

    private ObjectName(string value, string normalizedValue)
    {
        Value = value;
        NormalizedValue = normalizedValue;
    }

    public static ErrorOr<ObjectName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return DomainErrors.ObjectName.Required;
        }

        var normalizedValue = value.AsNormalizedQueryable();
        var name = new ObjectName(value, normalizedValue);

        var validator = new NameValidator();
        var result = validator.Validate(name);

        return result.ToErrorOr(name);
    }

    public override string ToString()
    {
        return Value;
    }

    private sealed class NameValidator : AbstractValidator<ObjectName>
    {
        public NameValidator()
        {
            RuleFor(model => model.Value).NotEmpty()
                .WithError(DomainErrors.ObjectName.Required);
            RuleFor(model => model.Value).MaximumLength(MaxLength)
                .WithError(DomainErrors.ObjectName.TooLong);
        }
    }
}