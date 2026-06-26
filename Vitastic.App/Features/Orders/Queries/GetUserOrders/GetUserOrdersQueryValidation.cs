using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrders
{
    public sealed class GetUserOrdersQueryValidation:AbstractValidator<GetUserOrdersQuery>
    {
        public GetUserOrdersQueryValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("شناسه کاربر نمی‌تواند خالی باشد");
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بزرگتر از 0 باشد");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("تعداد آیتم‌ها در هر صفحه باید بزرگتر از 0 باشد");
        }
    }
}