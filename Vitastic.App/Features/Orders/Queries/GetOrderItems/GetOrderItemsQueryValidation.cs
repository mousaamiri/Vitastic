using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetOrderItems
{
    public sealed class GetOrderItemsQueryValidation : AbstractValidator<GetOrderItemsQuery>
    {
        public GetOrderItemsQueryValidation()
        {
            RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش معتبر نمی باشد.");
        }
    }
}