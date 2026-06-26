using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Wallets.Queries.CheckSufficient
{
    public sealed record CheckSufficientBalanceQuery(
        Guid WalletId,
        decimal Amount) : IQuery<bool>;
}
