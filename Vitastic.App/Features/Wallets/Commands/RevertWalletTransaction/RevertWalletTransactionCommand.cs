using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Wallets.Commands.RevertWalletTransaction
{
    public sealed record RevertWalletTransactionCommand(
        Guid WalletId,
        Guid TransactionId) : ICommand;
}
