using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.RefundOrder
{
    public sealed class RefundOrderCommandValidation : AbstractValidator<RefundOrderCommand>
    {
        public RefundOrderCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.RefundReason).MaximumLength(200).WithMessage("دلیل بازپرداخت باید حداکثر 200 کاراکتر باشد.");
        }
    }
}