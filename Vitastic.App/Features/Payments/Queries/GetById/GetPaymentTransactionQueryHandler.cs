using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Payments.Queries.GetById;

public sealed class GetPaymentTransactionQueryHandler(
    IPaymentTransactionQuery transactionRepository,
    IMapper mapper)
    : IQueryHandler<GetPaymentTransactionQuery, PaymentTransactionDto>
{
    public async Task<Result<PaymentTransactionDto>> Handle(GetPaymentTransactionQuery query, CancellationToken cancellationToken)
    {
        var transactionIdResult = PaymentTransactionId.CreateFrom(query.TransactionId);
        if (transactionIdResult.IsFailure)
            return transactionIdResult.Error;
        PaymentTransactionDto? transaction = await transactionRepository.GetByIdAsync(transactionIdResult.Value, cancellationToken);
        if (transaction is null)
            return Error.NotFound("GetPaymentTransactionQuery.TransactionNotFound", "تراکنشی با این شناسه یافت نشد.");
        return mapper.Map<PaymentTransactionDto>(transaction);
    }
}
