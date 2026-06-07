using FluentValidation;
using Pulse.Domain.Common.Services;

namespace Pulse.App.Common.Validation;

public static class Validators
{
    public static IRuleBuilderOptions<T, string?> ValidIdentity<T>(
        this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(IdentityProvider.IsValid)
            .WithMessage("Invalid identity format.");
    }
}