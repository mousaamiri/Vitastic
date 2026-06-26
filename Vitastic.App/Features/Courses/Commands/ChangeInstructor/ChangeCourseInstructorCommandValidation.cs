using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.ChangeInstructor
{
    public sealed class ChangeCourseInstructorCommandValidation : AbstractValidator<ChangeCourseInstructorCommand>
    {
        public ChangeCourseInstructorCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.InstructorId)
                .NotEqual(Guid.Empty).WithMessage("شناسه مدرس نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه مدرس نمی تواند خالی باشد.");
        }
    }
}