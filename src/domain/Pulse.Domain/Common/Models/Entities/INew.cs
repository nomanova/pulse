namespace Pulse.Domain.Common.Models.Entities;

public interface INew<out T> where T : INew<T>
{
    static abstract T New(string value);
}