using System;
using System.Threading;
using System.Threading.Tasks;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Dto.Common;

namespace Pulse.App.Handlers.Applications.Commands;

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