using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.Search
{
    public  class SearchOrdersQueryValidation:AbstractValidator<SearchOrdersQuery>
    {
        public SearchOrdersQueryValidation()
        {
            RuleFor(x => x.SearchTerm)
                .NotEmpty().WithMessage("عبارت جستجو نمی‌تواند خالی باشد")
                .MaximumLength(100).WithMessage("عبارت جستجو نمی‌تواند بیشتر از 100 کاراکتر باشد");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
        }
    }
}