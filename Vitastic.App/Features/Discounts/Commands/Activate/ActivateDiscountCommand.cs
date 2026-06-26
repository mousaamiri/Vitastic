using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.Activate
{
    public sealed record ActivateDiscountCommand(
        Guid DiscountId) : ICommand;
}