using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Payments.Dtos;
using Vitastic.Domain.Entities.Transactions.Enums;

namespace Vitastic.App.Features.Payments.Commands.Init
{
    public sealed record InitializePaymentCommand(
        decimal Amount,
        TransactionType TransactionType,
        Guid? WalletId = null,
        Guid? OrderId = null,
        string? Description = null,
        string? CallbackUrl = null) : ICommand<InitializePaymentResult>;
}
