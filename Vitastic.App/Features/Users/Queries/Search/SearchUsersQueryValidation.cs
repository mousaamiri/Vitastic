using FluentValidation;

namespace Vitastic.App.Features.Users.Queries.Search;

public sealed class SearchUsersQueryValidation : AbstractValidator<SearchUsersQuery>
{
    public SearchUsersQueryValidation()
    {
        RuleFor(x => x.SearchTerm)
            .MinimumLength(1).WithMessage("عبارت جستجو باید حداقل 1 کاراکتر باشد")
            .MaximumLength(100).WithMessage("عبارت جستجو نمی‌تواند بیش از 100 کاراکتر باشد");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
    }
}
