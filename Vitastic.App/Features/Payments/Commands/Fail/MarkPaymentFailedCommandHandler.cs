using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.Fail
{
    public sealed class MarkPaymentFailedCommandHandler(IPaymentTransactionRepository repository) : ICommandHandler<MarkPaymentFailedCommand>
    {
       
        public async Task<Result> Handle(MarkPaymentFailedCommand request, CancellationToken cancellationToken)
        {
            var payentIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (payentIdResult.IsFailure)
                return payentIdResult.Error;
            var transaction = await repository.FindAsync(payentIdResult.Value, cancellationToken);
            if (transaction is null)
                return Error.NotFound("MarkPaymentFailedCommand.TransactionNotFound", "تراکنش مورد نظر یافت نشد.");

            transaction.MarkFailed();
            await repository.UpdateAsync(transaction, cancellationToken);
            return Result.Success();
        }
    }
}