using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Wallets.Commands.CompleteWalletTransaction
{
    public sealed record CompleteWalletTransactionCommand(
        Guid WalletId,
        Guid TransactionId) : ICommand;
}
