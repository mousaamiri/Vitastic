using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class PaymentTransactionQuery(
    string connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    ILogger<PaymentTransactionQuery> logger) : IPaymentTransactionQuery
{
    public async Task<PaymentTransactionDto?> GetByIdAsync(PaymentTransactionId value, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await readDbContext.PaymentTransactions
            .FirstOrDefaultAsync(t => t.Id == value, cancellationToken);
            return entity is null ? null : mapper.Map<PaymentTransactionDto>(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetByIdAsync");
            throw;
        }
    }

    public async Task<PaymentTransactionStatusDto?> GetPaymentStatus(PaymentTransactionId paymentIdValue, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await readDbContext.PaymentTransactions
            .FirstOrDefaultAsync(t => t.Id == paymentIdValue, cancellationToken);
            return entity is null ? null : mapper.Map<PaymentTransactionStatusDto>(entity);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in GetPaymentStatus");
            throw;
        }
    }
}
