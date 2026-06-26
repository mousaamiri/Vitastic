using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Commands.AssignPaymentToOrder
{
    public sealed class AssignPaymentToOrderCommandHandler(
        IPaymentTransactionRepository paymentRepository,
        IOrderRepository orderRepository) : ICommandHandler<AssignPaymentToOrderCommand>
    {

        public async Task<Result> Handle(AssignPaymentToOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Retrieve the payment transaction
            var transactionId  = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionId.IsFailure)                    return transactionId.Error;
            var transaction = await paymentRepository.FindAsync(transactionId.Value, cancellationToken);
            if (transaction == null)
            {
                throw new Exception("تراکنش پرداخت یافت نشد.");
            }

            // 2. Retrieve the order
            var orderId = OrderId.CreateFrom(request.OrderId);
            if (orderId.IsFailure)  
                return orderId.Error;
            bool isExist = await orderRepository.IsExistAsync(orderId.Value, cancellationToken);
            if (!isExist)
            {
                return Error.NotFound("AssignPaymentToOrderCommand.OrderNotFound", "سفارش یافت نشد.");
            }

            // 3. Associate the payment with the order
            transaction.AssignToOrder(orderId.Value);

            // 4. Update the payment transaction
            await paymentRepository.UpdateAsync(transaction, cancellationToken);
            return Result.Success();
        }
    }
}