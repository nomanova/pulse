using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.Infra.Database.Messaging.Queues;

public interface IQueue
{
    Task Send(List<Enqueueable> items, CancellationToken cancellationToken = default);
    
    Task StartReceiving(IQueueConsumer consumer);

    Task StopReceiving(IQueueConsumer consumer);
}