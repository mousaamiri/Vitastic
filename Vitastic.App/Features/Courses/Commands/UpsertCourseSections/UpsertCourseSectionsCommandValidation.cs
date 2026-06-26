using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.UpsertCourseSections
{
    public sealed class UpsertCourseSectionsCommandValidation : AbstractValidator<UpsertCourseSectionsCommand>
    {
        public UpsertCourseSectionsCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("شناسه دوره الزامی است");

            RuleFor(x => x.Sections)
                .NotNull()
                .WithMessage("لیست بخش‌ها نمی‌تواند خالی باشد")
                .Must(sections => sections.Count > 0)
                .WithMessage("حداقل یک بخش باید وجود داشته باشد");

            RuleForEach(x => x.Sections)
                .SetValidator(new SectionDtoValidator());

            // بررسی یکتا بودن DisplayOrder در سطح Section
            RuleFor(x => x.Sections)
                .Must(sections => sections.Select(s => s.DisplayOrder).Distinct().Count() == sections.Count)
                .WithMessage("ترتیب نمایش بخش‌ها نباید تکراری باشد")
                .When(x => x.Sections != null && x.Sections.Count > 0);
        }
    }
}