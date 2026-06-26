using FluentValidation;

namespace Vitastic.App.Features.Instructors.Commands.DeActive;

public sealed class DeActiveInstructorCommandValidation : AbstractValidator<DeActiveInstructorCommand>
{
    public DeActiveInstructorCommandValidation()
    {
        RuleFor(x => x.InstructorId)
            .NotEmpty()
            .WithMessage("شناسه استاد معتبر نیست.");
    }
}
