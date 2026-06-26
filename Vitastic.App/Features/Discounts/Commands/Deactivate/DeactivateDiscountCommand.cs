using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.Deactivate
{
    public sealed record DeactivateDiscountCommand(
        Guid DiscountId) : ICommand;
}