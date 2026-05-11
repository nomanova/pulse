namespace Pulse.Domain.Common.Models.Entities;

public interface IVersioned
{
    public uint Version { get; set; }
}