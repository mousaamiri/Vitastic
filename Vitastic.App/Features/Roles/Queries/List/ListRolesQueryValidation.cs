using FluentValidation;

namespace Vitastic.App.Features.Roles.Queries.List
{
    public sealed class ListRolesQueryValidation : AbstractValidator<ListRolesQuery>
    {
        public ListRolesQueryValidation()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0).WithMessage("شماره صفحه باید بیشتر از 0 باشد");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("تعداد آیتم‌ها باید بین 1 و 100 باشد");
        }
    }
}