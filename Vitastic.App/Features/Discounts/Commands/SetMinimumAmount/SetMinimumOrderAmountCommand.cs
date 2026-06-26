using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.SetMinimumAmount
{
    public sealed record SetMinimumOrderAmountCommand(
        Guid DiscountId,
        decimal Amount,
        string? Currency="IRT") : ICommand;
}
