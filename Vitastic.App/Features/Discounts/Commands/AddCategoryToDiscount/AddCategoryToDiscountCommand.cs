using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Discounts.Commands.AddCategoryToDiscount
{
    public sealed record AddCategoryToDiscountCommand(
        Guid DiscountId,
        Guid CategoryId) : ICommand;
}