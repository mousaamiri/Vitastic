using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.ClearOrderItems
{
    public sealed class ClearOrderItemsCommandValidation : AbstractValidator<ClearOrderItemsCommand>
    {
    
        public ClearOrderItemsCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEmpty().WithMessage("شناسه سفارش معتبر نیست.");
        }
    }
}
