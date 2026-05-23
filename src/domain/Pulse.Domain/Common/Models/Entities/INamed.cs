namespace Pulse.Domain.Common.Models.Entities;

public interface INamed
{
    string Name { get; }
    
    string NormalizedName { get; }
}