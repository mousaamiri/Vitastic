using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions.ValueObjects;

namespace Vitastic.App.Common.Abstractions.Services.Queries;

public interface IPaymentTransactionQuery
{
    Task<PaymentTransactionDto?> GetByIdAsync(PaymentTransactionId value, CancellationToken cancellationToken);
    Task<PaymentTransactionStatusDto?> GetPaymentStatus(PaymentTransactionId paymentIdValue, CancellationToken cancellationToken);
}
