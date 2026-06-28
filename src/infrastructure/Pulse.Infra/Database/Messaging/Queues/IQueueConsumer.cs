using System.Threading.Tasks;

namespace Pulse.Infra.Database.Messaging.Queues;

public interface IQueueConsumer
{
    Task OnMessageReceived(Dequeueable? item);
}