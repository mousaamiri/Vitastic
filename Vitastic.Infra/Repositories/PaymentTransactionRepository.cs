using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class PaymentTransactionRepository (ApplicationWriteDbContext dbContext):
    BaseRepository<PaymentTransaction, PaymentTransactionId>(dbContext), IPaymentTransactionRepository
{

    public async Task<PaymentTransaction?> GetByAuthorityAsync(string authority, CancellationToken cancellation = default) =>
        await ExecuteAsync(async () =>
            await Context.PaymentTransactions.FirstOrDefaultAsync(t =>
                t.PaymentInfo.Authority == authority, cancellation), cancellation);

    public async Task<PaymentTransaction?> GetPendingTransactionAsync(CancellationToken cancellation = default) =>
        await ExecuteAsync(async () =>
            await Context.PaymentTransactions.FirstOrDefaultAsync(t =>
                t.Status == TransactionStatus.Pending, cancellation), cancellation);

    public async Task<bool> WalletExistsAsync(WalletId walletId, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () => await Context.Wallets.AnyAsync(t => t.Id == walletId,
            cancellationToken: cancellationToken), cancellationToken);
    }
    public async Task<bool> ExistsAsync(InstructorId instructorId, CancellationToken cancellationToken)
        => await ExecuteAsync(async () => await Context.Instructors.AnyAsync(i => i.Id == instructorId, cancellationToken), cancellationToken);

    public async Task<PaymentTransaction?> IsExistsAsync(GuidBasedId<PaymentTransactionId> value, CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () => await Context.PaymentTransactions.FirstOrDefaultAsync(t => t.Id == value, cancellationToken), cancellationToken);
    }
}
