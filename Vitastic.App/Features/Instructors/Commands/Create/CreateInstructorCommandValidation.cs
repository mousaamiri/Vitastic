using FluentValidation;
using Vitastic.Domain.Entities.Instructors.ValueObjects;

namespace Vitastic.App.Features.Instructors.Commands.Create;

public sealed class CreateInstructorCommandValidation : AbstractValidator<CreateInstructorCommand>
{
    public CreateInstructorCommandValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("شناسه کاربر معتبر نیست.");

        RuleFor(x => x.Bio)
            .NotEmpty()
            .WithMessage("بیوگرافی نمیتواند خالی باشد.")
            .MinimumLength(InstructorBio.MinLength)
            .WithMessage($"بیوگرافی باید حداقل {InstructorBio.MinLength} کاراکتر باشد.")
            .MaximumLength(InstructorBio.MaxLength)
            .WithMessage($"بیوگرافی نمیتواند بیشتر از {InstructorBio.MaxLength} کاراکتر باشد.");
    }
}
