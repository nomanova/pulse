using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pulse.App.Common.Database;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.Users.Common.Specifications;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.Users.Services;
using Pulse.Domain.Aggregates.Users.ValueObjects;

namespace Pulse.Infra.Database.Seeders;

/**
 * Ensures the system admin exists.
 */
public static class AdminSeeder
{
    private const string ServerOwnerUsername = "admin";
    private const string ServerOwnerPasswordEnvVar = "PULSE_ADMIN_PASSWORD";

    private const int GeneratedPasswordLength = 24;

    private const string PasswordCharacters =
        "ABCDEFGHJKLMNPQRSTUVWXYZ" +
        "abcdefghijkmnopqrstuvwxyz" +
        "23456789" +
        "!@#$%^&*-_=+?";

    public static async Task Seed(IServiceProvider sp)
    {
        var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger(typeof(AdminSeeder));
        
        var unitOfWork = sp.GetRequiredService<IUnitOfWork>();
        var passwordHasher = sp.GetRequiredService<IUserPasswordHasher>();
        var userRepository = sp.GetRequiredService<IUserRepository>();
        var membershipRepository = sp.GetRequiredService<IMembershipRepository>();

        var specification = UserByUsernameSpecification.New(ServerOwnerUsername);
        var existingAdmin = await userRepository.SearchOne(specification);

        if (existingAdmin != null)
        {
            return;
        }

        // Create admin user
        var admin = User.Create(ServerOwnerUsername);
        admin.SetPassword(GetPassword(logger), passwordHasher);

        userRepository.Add(admin);

        // Make admin the (first) server owner
        var membership = Membership.Create(admin, Role.BuiltIn.SrvOwner);
        membershipRepository.Add(membership);

        await unitOfWork.Commit();
    }

    private static string GetPassword(ILogger logger)
    {
        var password = Environment.GetEnvironmentVariable(ServerOwnerPasswordEnvVar);

        if (string.IsNullOrEmpty(password))
        {
            password = GeneratePassword();

            logger.LogWarning(
                "No {PasswordEnvironmentVariable} environment variable was provided. " +
                "Generated temporary admin password: {Password}",
                ServerOwnerPasswordEnvVar,
                password);
        }

        return password;
    }

    private static string GeneratePassword(int length = GeneratedPasswordLength)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, Password.MinLength);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(length, Password.MaxLength);

        return RandomNumberGenerator.GetString(PasswordCharacters, length);
    }
}