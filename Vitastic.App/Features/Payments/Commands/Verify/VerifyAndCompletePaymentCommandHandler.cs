using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.Verify
{
    public sealed class VerifyAndCompletePaymentCommandHandler(
        IPaymentTransactionRepository transactionRepository,
        IPaymentGatewayService gatewayService)
        : ICommandHandler<VerifyAndCompletePaymentCommand, PaymentVerificationResult>
    {
        public async Task<Result<PaymentVerificationResult>> Handle(VerifyAndCompletePaymentCommand command, CancellationToken cancellationToken)
        {
            // 1. Find the transaction based on Authority
            var transaction = await transactionRepository.GetByAuthorityAsync(
                command.Authority,
                cancellationToken);
            if (transaction is null)
                return Error.NotFound("VerifyAndCompletePaymentCommand.TransactionNotFound",
                    "تراکنش یافت نشد.");

            // 2. Infrastructure: Vefify the payment with the gateway
            var verifyResult = await gatewayService.VerifyPaymentAsync(
                transaction,
                command.Status);

            if (verifyResult.IsFailure)
            {
                // If the confirmation failed, mark the transaction as Failed
                var failResult = transaction.MarkFailed();
                if (failResult.IsSuccess)
                {
                    await transactionRepository.UpdateAsync(transaction, cancellationToken);
                }

                return verifyResult.Error;
            }

            var refId = verifyResult.Value;

            // 3. Complete Order Or Wallet Transaction
            var completeResult = transaction.MarkCompleted(refId);

            if (completeResult.IsFailure)
                return completeResult.Error;

            // 4. Persist
            await transactionRepository.UpdateAsync(transaction, cancellationToken);
            return new PaymentVerificationResult(
                TransactionId: transaction.Id.Value,
                IsSuccess: true,
                RefId: refId
            );
        }
    }
}
