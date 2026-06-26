using FluentValidation;

namespace Vitastic.App.Features.Orders.Commands.AddItemToOrder
{
    public sealed class AddItemToOrderCommandValidation : AbstractValidator<AddItemToOrderCommand>
    {
        public AddItemToOrderCommandValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره معتبر نمی باشد.");
        }
    }
}