using FluentValidation;

namespace Vitastic.App.Features.Courses.Queries.Search
{
    public sealed class SearchCoursesQueryValidation : AbstractValidator<SearchCoursesQuery>
    {
        public SearchCoursesQueryValidation()
        {
            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("شماره صفحه باید بزرگتر یا مساوی 1 باشد.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("تعداد آیتم در صفحه باید بزرگتر یا مساوی 1 باشد.")
                .LessThanOrEqualTo(100).WithMessage("تعداد آیتم در صفحه نمی تواند بیشتر از 100 باشد.");

            // Search term validation (if provided)
            When(x => !string.IsNullOrWhiteSpace(x.SearchTerm), () =>
            {
                RuleFor(x => x.SearchTerm!)
                    .MinimumLength(2).WithMessage("عبارت جستجو باید حداقل 2 کاراکتر باشد.")
                    .MaximumLength(100).WithMessage("عبارت جستجو نمی تواند بیشتر از 100 کاراکتر باشد.");
            });

            // Enum validations
            When(x => x.Level.HasValue, () =>
            {
                RuleFor(x => x.Level!.Value)
                    .IsInEnum().WithMessage("سطح دوره معتبر نیست.");
            });

            When(x => x.Status.HasValue, () =>
            {
                RuleFor(x => x.Status!.Value)
                    .IsInEnum().WithMessage("وضعیت دوره معتبر نیست.");
            });
        }
    }
}
