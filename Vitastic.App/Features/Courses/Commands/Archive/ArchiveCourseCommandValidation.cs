using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.Archive
{
    public sealed class ArchiveCourseCommandValidation : AbstractValidator<ArchiveCourseCommand>
    {
        public ArchiveCourseCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}