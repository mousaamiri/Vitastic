using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Wallets.Dtos;

namespace Vitastic.App.Features.Wallets.Queries.GetBalance
{
    public sealed record GetWalletBalanceQuery(
        Guid WalletId) : IQuery<WalletBalanceDto>;
}
