using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Wallets.Commands.WithdrawFunds
{
    public sealed class WithdrawFundsCommandHandler(IWalletRepository walletRepository,
        IPaymentTransactionRepository transactionRepository) : ICommandHandler<WithdrawFundsCommand,Guid>
    {
        public async Task<Result<Guid>> Handle(WithdrawFundsCommand request, CancellationToken cancellationToken)
        {
            var walletIdResult = WalletId.CreateFrom(request.WalletId);
            if (walletIdResult.IsFailure)
                return walletIdResult.Error;
            var wallet = await walletRepository.FindAsync(walletIdResult.Value, cancellationToken);
            if (wallet is null)
                return Error.NotFound("WithdrawFundsCommand.WalletNotFound", "کیف پولی با این شناسه یافت نشد.");

            var transactionResult = wallet.WithdrawFunds(request.Amount, request.Description);
            if (transactionResult.IsFailure)
                return transactionResult.Error;

            var transaction = transactionResult.Value;

            await walletRepository.UpdateAsync(wallet, cancellationToken);
            await transactionRepository.AddAsync(transaction, cancellationToken);

            return transaction.Id.Value;
        }
    }
}
