using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Orders.Commands.SetTaxAmount;

public sealed record SetTaxAmountCommand(
    Guid OrderId,
    decimal TaxAmount) : ICommand;
