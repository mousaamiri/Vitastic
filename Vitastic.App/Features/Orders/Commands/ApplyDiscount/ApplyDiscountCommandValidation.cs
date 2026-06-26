using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.ApplyDiscount
{
    public sealed class ApplyDiscountCommandValidation : AbstractValidator<ApplyDiscountCommand>
    {
        public ApplyDiscountCommandValidation()
        {
            RuleFor(o => o.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نیست.");
            RuleFor(o => o.DiscountId).NotEqual(Guid.Empty).WithMessage("شناسه تخفیف معتبر نیست");
        }
    }
}