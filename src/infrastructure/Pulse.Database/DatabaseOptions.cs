using System.ComponentModel.DataAnnotations;

namespace Pulse.Database;

public record BaseDatabaseOptions
{
    [Required] public string? ConnectionString { get; init; }
    
    public DatabaseProvider Provider { get; init; } = DatabaseProvider.Postgres;
}

public sealed record DatabaseOptions : BaseDatabaseOptions
{
    public const string Section = "Database";
    
    public string? Schema { get; init; }
    
    public bool WithSensitiveDataLogging { get; init; }
}