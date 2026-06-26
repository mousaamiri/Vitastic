using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Transactions.Enums;

namespace Vitastic.App.Features.Payments.Commands.Create
{
    public sealed record CreatePaymentTransactionCommand(
        decimal Amount,
        TransactionType TransactionType,
        Guid? WalletId = null,
        string? Description = null) : ICommand<Guid>;
}
