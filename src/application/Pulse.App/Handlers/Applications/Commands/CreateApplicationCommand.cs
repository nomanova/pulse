using ErrorOr;
using FluentValidation;
using Pulse.App.Common.Authorization.Policies;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Validation;
using Pulse.App.Dto.Common;
using Pulse.Domain.Aggregates.Organizations;

namespace Pulse.App.Handlers.Applications.Commands;

public sealed record CreateApplicationCommand :
    IOrganizationScoped<string?>, ICommand<ErrorOr<IdentityDto>>
{
    public string? OrganizationId { get; init; }

    public string? Name { get; init; }
}

public sealed class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator()
    {
        RuleFor(command => command.OrganizationId).ValidIdentity();
    }
}

public sealed class CreateApplicationCommandAuthorizer :
    PermissionAuthorizer<CreateApplicationCommand>;