using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Pulse.Web.Core.Helpers;
using Pulse.Web.Core.Services.Interfaces;
using Pulse.Web.Core.ViewModels.Common;

namespace Pulse.Web.Core.ViewModels;

public sealed record SignInModel
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}

public sealed class SignInModelValidator : BaseModelValidator<SignInModel>
{
    public SignInModelValidator()
    {
        RuleFor(model => model.Username).NotEmpty().WithMessage("Username must not be empty");
        RuleFor(model => model.Password).NotEmpty().WithMessage("Password must not be empty");
    }
}

public partial class SignInViewModel : ViewModelBase
{
    private const string ErrInvalidCredentials = "Invalid credentials";

    public readonly SignInModelValidator ModelValidator = new();

    private readonly IPageNavigator _navigator;
    private readonly IAuthenticationService _authenticationService;

    public SignInViewModel(
        IPageNavigator navigator,
        IAuthenticationService authenticationService)
    {
        _navigator = navigator;
        _authenticationService = authenticationService;
    }

    protected override string Subtitle => "Sign In";
    
    [ObservableProperty] public partial SignInModel Model { get; private set; } = new();

    [ObservableProperty] public partial bool IsLoading { get; private set; }

    [ObservableProperty] public partial bool IsFailed { get; private set; }

    [ObservableProperty] public partial string? ErrorMessage { get; private set; }

    [ObservableProperty] public partial bool IsPasswordShown { get; private set; }

    [RelayCommand]
    private async Task OnSubmit()
    {
        IsFailed = false;
        IsLoading = true;

        var success = await _authenticationService.SignIn(Model.Username, Model.Password);

        IsLoading = false;

        if (!success)
        {
            SetFailed(ErrInvalidCredentials);
            return;
        }

        _navigator.NavigateTo(Routes.AppHome, replace: true);
    }

    [RelayCommand]
    private static void OnFieldChanged()
    {
        // NOP
    }

    [RelayCommand]
    private void OnTogglePassword()
    {
        IsPasswordShown = !IsPasswordShown;
    }

    private void SetFailed(string message)
    {
        ErrorMessage = message;
        IsFailed = true;
    }
}