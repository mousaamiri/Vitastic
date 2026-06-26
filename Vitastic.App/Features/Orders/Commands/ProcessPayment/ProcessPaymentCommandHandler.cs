using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.Enums;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Orders.Commands.ProcessPayment
{
    public sealed class ProcessPaymentCommandHandler(IPaymentTransactionRepository transactionRepository,
        IOrderRepository orderRepository) : ICommandHandler<ProcessPaymentCommand>
    {
        public async Task<Result> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            //Payment transaction id
            var transactionIdResult = PaymentTransactionId.CreateFrom(request.TransactionId);
            if (transactionIdResult.IsFailure)
                return transactionIdResult.Error;
            //Get transaction
            PaymentTransaction? transaction = await transactionRepository.IsExistsAsync(transactionIdResult.Value, cancellationToken);
            if (transaction is null)
                return Error.NotFound("ProcessPaymentCommand.TransactionNotFound", "تراکنشی با این شناسه یافت نشد. ");
            //Order id
            var orderIdResult = OrderId.CreateFrom(request.OrderId);
            if (orderIdResult.IsFailure)
                return orderIdResult.Error;
            //Get order
            var order = await orderRepository.FindAsync(orderIdResult.Value, cancellationToken);
            if (order is null)
                return Error.NotFound("ProcessPaymentCommand.OrderNotFound", "سفارشی با این شناسه یافت نشد. ");
            order.ProcessPayment(transactionIdResult.Value ,(PaymentMethod)request.PaymentMethodResponse);

            //Update transaction status based on payment result
            await transactionRepository.UpdateAsync(transaction, cancellationToken);

            return Result.Success();
        }
    }
}
