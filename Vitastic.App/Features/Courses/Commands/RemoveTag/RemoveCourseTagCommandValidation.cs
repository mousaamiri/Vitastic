using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.RemoveTag
{
    public sealed class RemoveCourseTagCommandValidation : AbstractValidator<RemoveCourseTagCommand>
    {
        public RemoveCourseTagCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.TagId)
                .NotEmpty().WithMessage("شناسه برچسب نمی تواند خالی باشد.");
        }
    }
}