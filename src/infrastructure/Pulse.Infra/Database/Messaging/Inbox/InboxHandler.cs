using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Services.Interfaces;
using Pulse.Infra.Database.Contexts;

namespace Pulse.Infra.Database.Messaging.Inbox;

public sealed class InboxHandler : IInbox
{
    private readonly DatabaseContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public InboxHandler(
        DatabaseContext context,
        IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task ExecuteIdempotently(
        string notificationId,
        string handler,
        Func<CancellationToken, Task> handle,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var inboxMessage = await _context.InboxMessages
            .SingleOrDefaultAsync(
                message =>
                    message.Id == notificationId &&
                    message.Handler == handler,
                cancellationToken);

        if (inboxMessage?.ProcessedOn is not null)
        {
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        if (inboxMessage is null)
        {
            inboxMessage = new InboxMessage
            {
                Id = notificationId,
                Handler = handler,
                ReceivedOn = _dateTimeProvider.UtcNow
            };

            _context.InboxMessages.Add(inboxMessage);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync(CancellationToken.None);

                await using var retryTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                inboxMessage = await _context.InboxMessages
                    .SingleAsync(
                        message =>
                            message.Id == notificationId &&
                            message.Handler == handler,
                        cancellationToken);

                if (inboxMessage.ProcessedOn is not null)
                {
                    await retryTransaction.CommitAsync(cancellationToken);
                    return;
                }

                await retryTransaction.RollbackAsync(CancellationToken.None);

                throw;
            }
        }

        try
        {
            await handle(cancellationToken);

            inboxMessage.ProcessedOn = _dateTimeProvider.UtcNow;
            inboxMessage.Error = null;

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            inboxMessage.Error = ex.ToString();

            await _context.SaveChangesAsync(CancellationToken.None);
            await transaction.RollbackAsync(CancellationToken.None);

            throw;
        }
    }
}