using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.Cancel
{
    public sealed class CancelPaymentCommandHandler(IPaymentTransactionRepository paymentRepository)
        : ICommandHandler<CancelPaymentCommand>
    {
        public async Task<Result> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the payment transaction
            var transactionId = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionId.IsFailure)
                return transactionId.Error;
            var transaction = await paymentRepository.FindAsync(transactionId.Value, cancellationToken);
            if (transaction == null)
                return Error.NotFound("CancelPaymentCommand.TransactionNotFound", "تراکنش پرداختی با این شناسه یافت نشد.");

            // 2. Mark as cancelled
            var cancelResult = transaction.Cancel();
            if (cancelResult.IsFailure)
            {
                return cancelResult.Error;
            }

            // 3. Persist
            await paymentRepository.UpdateAsync(transaction, cancellationToken);

            return Result.Success();
        }
    }
}
