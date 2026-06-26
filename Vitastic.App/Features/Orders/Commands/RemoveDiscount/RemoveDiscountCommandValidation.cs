using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.RemoveDiscount
{
    public sealed class RemoveDiscountCommandValidation : AbstractValidator<RemoveDiscountCommand>
    {
        public RemoveDiscountCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
        }
    }
}