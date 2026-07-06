using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Common.Models.Entities;

public static class EntityIdExtensions
{
    public static T AsIdentity<T>(this string? id) where T : EntityId, INew<T>
    {
        if (!IdentityProvider.IsValid(id))
        {
            DomainErrors.InvalidIdentity.Throw();
        }

        return T.New(id!);
    }
}