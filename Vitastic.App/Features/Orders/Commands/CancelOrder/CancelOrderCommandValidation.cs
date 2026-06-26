using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.CancelOrder
{
    public sealed class CancelOrderCommandValidation : AbstractValidator<CancelOrderCommand>
    {
        public CancelOrderCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.CancelReason).MaximumLength(100).WithMessage("دلیل لغو سفارش باید حداکثر 100 کاراکتر باشد.");
        }
    }
}