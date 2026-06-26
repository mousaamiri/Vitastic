using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.Revert
{
    public sealed class RevertPaymentCommandHandler(IPaymentTransactionRepository repository) : ICommandHandler<RevertPaymentCommand>
    {

        public async Task<Result> Handle(RevertPaymentCommand request, CancellationToken cancellationToken)
        {
            var payentIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (payentIdResult.IsFailure)
                return payentIdResult.Error;
            var transaction = await repository.FindAsync(payentIdResult.Value, cancellationToken);
            if (transaction is null)
                return Error.NotFound("RevertPaymentCommand.TransactionNotFound", "تراکنش مورد نظر یافت نشد.");

            transaction.Revert();
            await repository.UpdateAsync(transaction, cancellationToken);
            return Result.Success();
        }
    }
}