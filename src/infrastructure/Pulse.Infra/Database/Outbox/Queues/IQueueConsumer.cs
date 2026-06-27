using System.Threading.Tasks;

namespace Pulse.Infra.Database.Outbox.Queues;

public interface IQueueConsumer
{
    Task OnMessageReceived(Dequeueable? item);
}