using System.Threading.Tasks;

namespace Pulse.Database.Outbox.Queues;

public interface IQueueConsumer
{
    Task OnMessageReceived(Dequeueable? item);
}