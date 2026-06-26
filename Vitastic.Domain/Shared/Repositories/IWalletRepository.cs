using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Entities.Wallets;
using Vitastic.Domain.Entities.Wallets.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IWalletRepository:IRepository<Wallet,WalletId>
{
    Task<bool> CheckUserIsExist(UserId userId, CancellationToken token=default);
    Task<bool> CheckUserHasWalletAsync(UserId userId, CancellationToken token=default);
}
