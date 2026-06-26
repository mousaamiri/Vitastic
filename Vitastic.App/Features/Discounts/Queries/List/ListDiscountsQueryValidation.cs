using FluentValidation;
using Vitastic.App.Features.Tags.Queries.List;

namespace Vitastic.App.Features.Discounts.Queries.List;

public sealed class ListDiscountsQueryValidation : AbstractValidator<ListTagsQuery>
{
    public ListDiscountsQueryValidation()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
    }
}
