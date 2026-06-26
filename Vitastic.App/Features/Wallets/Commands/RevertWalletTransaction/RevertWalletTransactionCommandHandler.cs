using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Commands.RevertWalletTransaction
{
    public sealed class RevertWalletTransactionCommandHandler(
        IWalletRepository walletRepository,
        IPaymentTransactionRepository transactionRepository) : ICommandHandler<RevertWalletTransactionCommand>
    {
        public async Task<Result> Handle(RevertWalletTransactionCommand request, CancellationToken cancellationToken)
        {
            var walletIdResult = WalletId.CreateFrom(request.WalletId);
            if (walletIdResult.IsFailure) return walletIdResult.Error;
            var wallet = await walletRepository.FindAsync(
                walletIdResult.Value,
                cancellationToken);
            if (wallet is null)
                return Error.NotFound("RevertWalletTransaction.WalletNotFound", "این کیف پول یافت نشد.");

            Result<PaymentTransactionId> transactionIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionIdResult.IsFailure)
                return transactionIdResult.Error;
            PaymentTransaction? transaction = await transactionRepository.FindAsync(transactionIdResult.Value, cancellationToken);
            if (transaction is null)
                return Error.NotFound("RevertWalletTransaction.TransactionNotFound", "این تراکنش یافت نشده یا قبلا تکمیل شده.");

            var revertResult = wallet.RevertTransaction(transaction);

            if (revertResult.IsFailure)
                return revertResult.Error;

            await walletRepository.UpdateAsync(wallet, cancellationToken);
            await transactionRepository.UpdateAsync(transaction, cancellationToken);
            return Result.Success();
        }
    }
}
