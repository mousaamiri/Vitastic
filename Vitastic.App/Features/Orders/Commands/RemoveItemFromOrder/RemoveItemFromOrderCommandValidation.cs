using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.RemoveItemFromOrder
{
    public sealed class RemoveItemFromOrderCommandValidation : AbstractValidator<RemoveItemFromOrderCommand>
    {
        public RemoveItemFromOrderCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("شناسه سفارش معتبر نیست.");
            RuleFor(x => x.OrderItemId).NotEmpty().WithMessage("شناسه آیتم سفارش معتبر نیست.");
        }
    }
}