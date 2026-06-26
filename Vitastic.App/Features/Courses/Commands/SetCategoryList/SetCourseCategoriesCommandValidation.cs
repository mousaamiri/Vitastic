using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.SetCategoryList
{
    public sealed class SetCourseCategoriesCommandValidation : AbstractValidator<SetCourseCategoriesCommand>
    {
        public SetCourseCategoriesCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.CategoryIds)
                .NotNull().WithMessage("لیست دسته‌بندی‌ها نمی تواند خالی باشد.")
                .Must(x => x.Count == x.Distinct().Count()).WithMessage("لیست دسته‌بندی‌ها نمی تواند دارای مقادیر تکراری باشد.");
        }
    }
}