using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Wallets.Commands.AddFunds
{
    public sealed record AddFundsCommand(
        Guid WalletId,
        decimal Amount,
        string? Description = null) : ICommand<Guid>;
}


