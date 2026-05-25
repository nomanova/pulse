using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Database;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.Users.Common.Specifications;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.Services;

namespace Pulse.Infra.Database.Seeders;

public static class InitSeeder
{
    public static async Task Seed(IServiceProvider sp)
    {
        await SeedUser(sp, "admin", "admin@admin.com", "Admin123456");
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
}