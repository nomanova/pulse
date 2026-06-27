namespace Pulse.Tests.Shared;

public interface IBuilder<out T>
{
    T Build();
}