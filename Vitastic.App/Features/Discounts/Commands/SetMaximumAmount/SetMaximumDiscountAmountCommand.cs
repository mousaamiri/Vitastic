using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.SetMaximumAmount
{
    public sealed record SetMaximumDiscountAmountCommand(
        Guid DiscountId,
        decimal MaxAmount,
        string? Currency="IRT") : ICommand;
}
