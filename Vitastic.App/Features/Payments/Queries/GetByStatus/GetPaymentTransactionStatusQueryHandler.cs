using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Queries.GetByStatus;

public sealed class GetPaymentTransactionStatusQueryHandler(IPaymentTransactionQuery paymentRepository,
    IMapper mapper):
    IQueryHandler<GetPaymentTransactionStatusQuery, PaymentTransactionStatusDto>
{
    public async Task<Result<PaymentTransactionStatusDto>> Handle(GetPaymentTransactionStatusQuery query, CancellationToken cancellationToken)
    {
        //Transaction id
        var paymentId =  PaymentTransactionId.CreateFrom(query.TransactionId);
        if (paymentId.IsFailure)
            return paymentId.Error;

        //Find transaction by id
        PaymentTransactionStatusDto? transaction = await paymentRepository.GetPaymentStatus(paymentId.Value, cancellationToken);
        if (transaction is null)
            return Error.NotFound("GetPaymentTransactionStatusQuery.TransactionNotFound", "تراکنشی با این شناسه یافت نشد.");

        return transaction;
    }
}
