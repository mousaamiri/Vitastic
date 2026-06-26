using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.RemoveSection
{
    public sealed class RemoveCourseSectionCommandValidation : AbstractValidator<RemoveCourseSectionCommand>
    {
        public RemoveCourseSectionCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("شناسه بخش نمی تواند خالی باشد.");
        }
    }
}