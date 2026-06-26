using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.Unpublish
{
    public sealed class UnpublishCourseCommandValidation : AbstractValidator<UnpublishCourseCommand>
    {
        public UnpublishCourseCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}