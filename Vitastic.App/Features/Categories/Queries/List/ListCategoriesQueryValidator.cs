using FluentValidation;

namespace Vitastic.App.Features.Categories.Queries.List;

public sealed class ListCategoriesQueryValidator:AbstractValidator<ListCategoriesQuery>
{
    public ListCategoriesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
    }
}
