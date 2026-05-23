using System.ComponentModel.DataAnnotations;

namespace Pulse.Infra.Security.DataProtection;

public class DataProtectionOptions
{
    public const string Section = "DataProtection";
    
    public enum DataProtectionMode
    {
        Default,
        BlobWithKeyVault
    }
    
    public class BlobWithKeyVaultOptions
    {
        [Required] public string? ConnectionString { get; set; }

        [Required] public string? ContainerName { get; set; }

        [Required] public string? BlobName { get; set; }

        [Required] public string? VaultName { get; set; }

        [Required] public string? KeyName { get; set; }
    }
    
    [Required] public DataProtectionMode Mode { get; init; }

    public BlobWithKeyVaultOptions? BlobWithKeyVault { get; set; }
}