using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class WalletRepository(ApplicationWriteDbContext context) :
    BaseRepository<Wallet, WalletId>(context), IWalletRepository
{
    public new async Task<Wallet?> FindAsync(WalletId id, CancellationToken token = default)
        => await ExecuteAsync(async () =>
            {
                var wallet = await Context.Wallets
                    .FirstOrDefaultAsync(x => x.Id == id, token);

                if (wallet == null)
                    return wallet;

                await InitWalletTransactions(id, token, wallet);
                return wallet;
            }
            , token);

    private async Task InitWalletTransactions(WalletId id, CancellationToken token, Wallet wallet)
    {
        List<PaymentTransactionId> transactions = await Context.PaymentTransactions
            .Where(t => t.WalletId == id)
            .Select(t => t.Id)
            .ToListAsync(token);
        wallet.InitTransactionIds(transactions);
    }

    public async Task<bool> CheckUserIsExist(UserId userId, CancellationToken token=default)
        => await ExecuteAsync(
            async  () => await Context.Users.AnyAsync(x => x.Id == userId, token),token);

    public async Task<bool> CheckUserHasWalletAsync(UserId userId, CancellationToken token = default)
        => await ExecuteAsync(
            async  () => await Context.Wallets.AnyAsync(x => x.UserId == userId, token),token);

}
