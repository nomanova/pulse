namespace Pulse.Domain.Tests.Shared;

public interface IBuilder<out T>
{
    T Build();
}