using System.Transactions;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.Domain.Entities.Orders;

namespace Vitastic.App.Features.Orders.Commands.ProcessPayment;

public sealed record ProcessPaymentCommand(
    Guid OrderId,
    Guid TransactionId,
    PaymentMethodDto PaymentMethodResponse) : ICommand;
