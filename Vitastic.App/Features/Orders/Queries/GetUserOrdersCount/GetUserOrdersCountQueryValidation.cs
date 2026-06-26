using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrdersCount
{
    public sealed class GetUserOrdersCountQueryValidation:AbstractValidator<GetUserOrdersCountQuery>
    {
        public GetUserOrdersCountQueryValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی‌تواند خالی باشد");
        }
    }
}