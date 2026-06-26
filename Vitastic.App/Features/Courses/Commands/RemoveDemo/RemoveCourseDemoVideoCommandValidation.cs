using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.RemoveDemo
{
    public sealed class RemoveCourseDemoVideoCommandValidation : AbstractValidator<RemoveCourseDemoVideoCommand>
    {
        public RemoveCourseDemoVideoCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");
        }
    }
}