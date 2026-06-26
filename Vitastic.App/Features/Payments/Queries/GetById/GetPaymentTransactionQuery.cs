using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Payments.Dtos;

namespace Vitastic.App.Features.Payments.Queries.GetById;

public sealed record GetPaymentTransactionQuery(
    Guid TransactionId) : IQuery<PaymentTransactionDto>;
