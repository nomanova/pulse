using Pulse.Application.Common.Dispatcher;
using Pulse.Application.Dto.Common;

namespace Pulse.Application.Handlers.Applications.Commands;

public sealed record CreateApplicationCommand : ICommand<IdentityDto>
{
    public required string Name { get; init; }
}

public class CreateApplicationCommandHandler : ICommandHandler<CreateApplicationCommand, IdentityDto>
{
    public async ValueTask<IdentityDto> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(new IdentityDto
        {
            Id = Guid.NewGuid().ToString()
        });
    }
}