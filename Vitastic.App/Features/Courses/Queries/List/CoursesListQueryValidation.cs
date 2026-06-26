using FluentValidation;

namespace Vitastic.App.Features.Courses.Queries.List
{
    public sealed class CoursesListQueryValidation : AbstractValidator<CoursesListQuery>
    {
        public CoursesListQueryValidation()
        {
            // Pagination validation
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("شماره صفحه باید بزرگتر یا مساوی 1 باشد.");

            RuleFor(x => x.PageSize)
                .GreaterThanOrEqualTo(1).WithMessage("تعداد آیتم در صفحه باید بزرگتر یا مساوی 1 باشد.")
                .LessThanOrEqualTo(100).WithMessage("تعداد آیتم در صفحه نمی تواند بیشتر از 100 باشد.");

            RuleFor(x => x)
                .Must(x => x.UserId.HasValue || !string.IsNullOrWhiteSpace(x.SessionId))
                .WithMessage("شناسه کاربر یا شناسه نشست الزامی است");
        }
    }
}
