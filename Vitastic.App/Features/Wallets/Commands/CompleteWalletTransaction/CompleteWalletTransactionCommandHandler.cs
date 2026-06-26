using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Commands.CompleteWalletTransaction
{
    public sealed class CompleteWalletTransactionCommandHandler(
        IWalletRepository walletRepository,IPaymentTransactionRepository transactionRepository) : ICommandHandler<CompleteWalletTransactionCommand>
    {

        public async Task<Result> Handle(CompleteWalletTransactionCommand request, CancellationToken cancellationToken)
        {
            var walletIdResult = WalletId.CreateFrom(request.WalletId);
            if (walletIdResult.IsFailure)
                return walletIdResult.Error;
            var wallet = await walletRepository.FindAsync(walletIdResult.Value, cancellationToken);
            if (wallet is null)
                return Error.NotFound("CompleteWalletTransactionCommand.WalletNotFound", "کیف پولی با این شناسه یافت نشد.");

            // Complete the transaction
            var transactionIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionIdResult.IsFailure)
                return transactionIdResult.Error;
            //Get transaction from wallet transactions
            var transaction = await transactionRepository.FindAsync(transactionIdResult.Value, cancellationToken);
            if (transaction is null)                
                return Error.NotFound("CompleteWalletTransactionCommand.TransactionNotFound", "تراکنشی با این شناسه در کیف پول یافت نشد.");
            var completeResult = wallet.CompleteTransaction(transaction);
            if (completeResult.IsFailure)
                return completeResult.Error;

            await walletRepository.UpdateAsync(wallet, cancellationToken);
            return Result.Success();
        }
    }
}