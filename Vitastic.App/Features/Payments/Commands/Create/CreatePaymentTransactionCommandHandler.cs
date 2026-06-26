using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Wallets.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Payments.Commands.Create
{
    public class CreatePaymentTransactionCommandHandler(IPaymentTransactionRepository transactionRepository,IMapper mapper)
        : ICommandHandler<CreatePaymentTransactionCommand,Guid>
    {
        public async Task<Result<Guid>> Handle(CreatePaymentTransactionCommand command,
            CancellationToken cancellationToken)
        {
            var amount = Money.Create(command.Amount, "IRT").Value;


            WalletId? walletId = null;
            if (command.WalletId.HasValue)
            {
                var walletIdResult = WalletId.CreateFrom(command.WalletId.Value);
                if (walletIdResult.IsFailure)
                    return walletIdResult.Error;
                var isWalletExist =
                    await transactionRepository.WalletExistsAsync(walletIdResult.Value, cancellationToken);
                if (!isWalletExist)
                    return Error.NotFound("CreatePaymentTransactionCommand.WalletNotFound",
                        "کیف پولی با این شناسه یافت نشد.");
                walletId = walletIdResult.Value;
            }

            Result<PaymentTransaction> transactionResult = PaymentTransaction.Create(
                amount,
                command.TransactionType,
                command.Description,
                walletId: walletId);

            if (transactionResult.IsFailure)
                throw new InvalidOperationException(transactionResult.Error.Message);

            var transaction = transactionResult.Value;

            await transactionRepository.AddAsync(transaction, cancellationToken);
            return transaction.Id.Value;
        }
    }
}
