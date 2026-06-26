using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetById
{
    public sealed class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        public GetOrderByIdQueryValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEqual(Guid.Empty).WithMessage("شناسه سفارش نمی‌تواند خالی باشد");
        }
    }
}