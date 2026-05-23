using FluentValidation;
using FluentValidation.Results;
using Pulse.Domain.Common.Errors;

namespace Pulse.Domain.Common.Models.Text;

public class TextValidator : AbstractValidator<string?>
{
    private readonly string _property;

    public TextValidator(string property, uint maxLength, uint? minLength = null)
    {
        _property = property;

        RuleFor(text => text)
            .NotEmpty()
            .WithError(BusinessErrors.Text.Required(property));

        RuleFor(text => text)
            .MaximumLength((int)maxLength)
            .WithError(BusinessErrors.Text.TooLong(property, maxLength));

        if (minLength.HasValue)
        {
            RuleFor(text => text)
                .MinimumLength((int)minLength.Value)
                .WithError(BusinessErrors.Text.TooShort(property, minLength.Value));
        }
    }

    protected override bool PreValidate(ValidationContext<string?> context, ValidationResult result)
    {
        if (context.InstanceToValidate == null)
        {
            result.Errors.Add(new ValidationFailure(_property,
                BusinessErrors.Text.Required(_property).Description));
            return false;
        }

        return true;
    }
}