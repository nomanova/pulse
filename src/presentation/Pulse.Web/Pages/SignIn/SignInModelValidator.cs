using FluentValidation;
using Pulse.Web.Core.Helpers;

namespace Pulse.Web.Pages.SignIn;

public sealed class SignInModelValidator : BaseModelValidator<SignInModel>
{
    public SignInModelValidator()
    {
        RuleFor(model => model.Username).NotEmpty().WithMessage("Username must not be empty");
        RuleFor(model => model.Password).NotEmpty().WithMessage("Password must not be empty");
    }
}