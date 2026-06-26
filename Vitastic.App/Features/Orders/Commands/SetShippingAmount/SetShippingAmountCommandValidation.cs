using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.SetShippingAmount
{
    public sealed class SetShippingAmountCommandValidation : AbstractValidator<SetShippingAmountCommand>
    {
        public SetShippingAmountCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.ShippingAmount).GreaterThanOrEqualTo(0).WithMessage("مقدار هزینه ارسال نمی تواند منفی باشد.");
        }
    }
}