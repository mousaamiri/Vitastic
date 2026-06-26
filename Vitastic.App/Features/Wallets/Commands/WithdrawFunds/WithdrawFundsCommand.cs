using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Wallets.Commands.WithdrawFunds
{
    public sealed record WithdrawFundsCommand(
        Guid WalletId,
        decimal Amount,
        string? Description = null) : ICommand<Guid>;
}
