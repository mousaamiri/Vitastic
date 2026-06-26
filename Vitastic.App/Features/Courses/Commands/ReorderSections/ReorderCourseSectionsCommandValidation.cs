using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.ReorderSections
{
    public sealed class ReorderCourseSectionsCommandValidation : AbstractValidator<ReorderCourseSectionsCommand>
    {
        public ReorderCourseSectionsCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.OrderedSectionIds)
                .NotEmpty().WithMessage("لیست بخش‌ها نمی تواند خالی باشد.")
                .Must(x => x.Count == x.Distinct().Count()).WithMessage("لیست بخش‌ها نمی تواند دارای مقادیر تکراری باشد.");
        }
    }
}