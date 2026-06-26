using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.UpdateLevel
{
    public sealed class ChangeCourseLevelCommandValidation : AbstractValidator<ChangeCourseLevelCommand>
    {
        public ChangeCourseLevelCommandValidation()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.Level)
                .IsInEnum().WithMessage("سطح دوره معتبر نیست.");
        }
    }
}