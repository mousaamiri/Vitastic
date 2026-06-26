using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.AddCategory
{
    public sealed class AddCourseCategoryCommandValidation : AbstractValidator<AddCourseCategoryCommand>
    {
        public AddCourseCategoryCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("شناسه دسته‌بندی نمی تواند خالی باشد.");
        }
    }
}