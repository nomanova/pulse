using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pulse.App.Common.Dispatcher;

public interface IInbox
{
    Task ExecuteIdempotently(
        string notificationId,
        string handler,
        Func<CancellationToken, Task> handle,
        CancellationToken cancellationToken = default);
}