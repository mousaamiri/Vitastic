using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.AssignInfo
{
    public sealed class AssignPaymentInfoCommandHandler(IPaymentTransactionRepository paymentRepository, 
        IOrderRepository orderRepository) : ICommandHandler<AssignPaymentInfoCommand>
    {

        public async Task<Result> Handle(AssignPaymentInfoCommand request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the payment transaction
            var transactionId = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionId.IsFailure)
                return transactionId.Error;
            var transaction = await paymentRepository.FindAsync(transactionId.Value, cancellationToken);
            if (transaction == null)
                throw new Exception("تراکنش پرداخت یافت نشد.");

            transaction.AssignPaymentInfo(request.Authority, request.Provider);

            await paymentRepository.UpdateAsync(transaction, cancellationToken);
            return Result.Success();
        }

    }
}