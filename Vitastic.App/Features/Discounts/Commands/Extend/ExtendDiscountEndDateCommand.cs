using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.Extend
{
    public sealed record ExtendDiscountEndDateCommand(
        Guid DiscountId,
        DateTimeOffset NewEndDate) : ICommand;
}