using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Database;
using Pulse.App.Handlers.Roles.Common;
using Pulse.App.Handlers.Roles.Common.Specifications;
using Pulse.Domain.Aggregates.Roles;

namespace Pulse.Database.Seeders;

/**
 * Ensures the system roles exist.
 */
public static class BuiltInRoleSeeder
{
    public static async Task Seed(IServiceProvider sp)
    {
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var roleRepository = sp.GetRequiredService<IRoleRepository>();

        foreach (var role in Role.BuiltIn.All)
        {
            await UpsertRole(role, roleRepository);
        }

        await unitOfWork.Commit();
    }

    private static async Task UpsertRole(Role role, IRoleRepository roleRepository)
    {
        var existingRole = await roleRepository.SearchOne(new RoleByIdSpecification(role.Id));

        if (existingRole is null)
        {
            roleRepository.Add(role);
        }
    }
}