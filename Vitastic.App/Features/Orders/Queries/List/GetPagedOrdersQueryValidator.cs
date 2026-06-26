using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.List
{
    public sealed class GetPagedOrdersQueryValidator : AbstractValidator<GetPagedOrdersQuery>
    {
        public GetPagedOrdersQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
        }
    }
}