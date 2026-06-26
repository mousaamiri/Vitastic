using FluentValidation;
using Vitastic.Domain.Entities.Courses.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.AddSection
{
    public sealed class AddCourseSectionCommandValidation : AbstractValidator<AddCourseSectionCommand>
    {
        public AddCourseSectionCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان بخش نمی تواند خالی باشد.")
                .MaximumLength(SectionTitle.MaxLength).WithMessage($"عنوان بخش نمی تواند بیشتر از {SectionTitle.MaxLength} کاراکتر باشد.")
                .MinimumLength(SectionTitle.MinLength).WithMessage($"عنوان بخش نمی تواند کمتر از {SectionTitle.MinLength} کاراکتر باشد.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("ترتیب نمایش باید بزرگتر یا مساوی صفر باشد.");
        }
    }
}