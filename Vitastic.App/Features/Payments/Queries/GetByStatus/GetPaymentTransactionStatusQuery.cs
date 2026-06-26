using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Payments.Dtos;

namespace Vitastic.App.Features.Payments.Queries.GetByStatus;

public sealed record GetPaymentTransactionStatusQuery(
    Guid TransactionId) : IQuery<PaymentTransactionStatusDto>;
