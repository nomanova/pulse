using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Database;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Memberships.Common.Specifications;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Organizations.Common.Specifications;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.Users.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.Database.Seeders;

public static class DefaultOrgSeeder
{
    public static async Task Seed(IServiceProvider sp)
    {
        var user = await SeedUser(sp, "admin", "admin@admin.com", "Admin123456");
        await SeedOrganization(sp, "Pulse", user);
    }

    private static async Task<User> SeedUser(
        IServiceProvider sp, string username, string email, string password)
    {
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var passwordHasher = sp.GetRequiredService<IUserPasswordHasher>();
        var userRepository = sp.GetRequiredService<IUserRepository>();

        var existingUser = await userRepository.SearchOne(UserByEmailAddressSpecification.New(email));

        if (existingUser != null)
        {
            return existingUser;
        }

        var user = User.Create(username);

        user.SetEmail(email);
        user.SetPassword(password, passwordHasher);
        user.SetEmailConfirmed();

        userRepository.Add(user);
        await unitOfWork.Commit();

        return user;
    }

    private static async Task<Organization> SeedOrganization(IServiceProvider sp, string name, User user)
    {
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var organizationRepository = sp.GetRequiredService<IOrganizationRepository>();
        var membershipRepository = sp.GetRequiredService<IMembershipRepository>();

        var organization = await organizationRepository.SearchOne(new OrganizationByNameSpecification(name));

        if (organization == null)
        {
            organization = Organization.Create(name);
            organizationRepository.Add(organization);
        }

        var memberships = await membershipRepository.Search(
            new UserMembershipsByOrganizationSpecification(user.Id, organization.Id));

        var membership = memberships.FirstOrDefault(m => m.Scope == Scope.Organization &&
                                                         m.RoleId == Role.BuiltIn.OrgOwner.Id);

        if (membership == null)
        {
            membership = Membership.Create(user, Role.BuiltIn.OrgOwner, organization);
            membershipRepository.Add(membership);
        }

        await unitOfWork.Commit();

        return organization;
    }
}