using System;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulse.Database;
using Throw;

namespace Pulse.Infra.Security.DataProtection;

internal static class Setup
{
    public static IServiceCollection AddAppDataProtection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.ConfigureAndValidate<DataProtectionOptions>(DataProtectionOptions.Section, configuration);

        var dataProtectionOptions =
            configuration.GetSection(DataProtectionOptions.Section).Get<DataProtectionOptions>();

        dataProtectionOptions.ThrowIfNull();

        var mode = dataProtectionOptions.Mode;

        switch (mode)
        {
            case DataProtectionOptions.DataProtectionMode.Default:
                AddDefaultDataProtection(services);
                break;
            case DataProtectionOptions.DataProtectionMode.BlobWithKeyVault:
                AddBlobWithKeyVaultDataProtection(services, dataProtectionOptions);
                break;
            default:
                throw new NotImplementedException(mode.ToString());
        }

        return services;
    }

    private static void AddDefaultDataProtection(IServiceCollection services)
    {
        services.AddDataProtection();
    }

    // https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview
    // https://learn.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential
    private static void AddBlobWithKeyVaultDataProtection(IServiceCollection services,
        DataProtectionOptions options)
    {
        var protectionOptions = options.BlobWithKeyVault;
        protectionOptions.ThrowIfNull();

        var connectionString = protectionOptions.ConnectionString;
        var containerName = protectionOptions.ContainerName;
        var blobName = protectionOptions.BlobName;

        var keyName = protectionOptions.KeyName;
        var vaultName = protectionOptions.VaultName;

        var keyIdentifier = $"https://{vaultName}.vault.azure.net/keys/{keyName}/";

        services.AddDataProtection()
            .PersistKeysToAzureBlobStorage(connectionString, containerName, blobName)
            .ProtectKeysWithAzureKeyVault(new Uri(keyIdentifier), new DefaultAzureCredential());
    }
}