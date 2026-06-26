using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetPendingOrders
{
    public sealed class GetPendingOrdersQueryValidation:AbstractValidator<GetPendingOrdersQuery>
    {
        public GetPendingOrdersQueryValidation()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بزرگتر از 0 باشد");
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("تعداد آیتم‌ها در هر صفحه باید بزرگتر از 0 باشد");
        }
    }
}