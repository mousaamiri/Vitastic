using FluentValidation;

namespace Vitastic.App.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandValidation : AbstractValidator<DeactivateUserCommand>
    {
        public DeactivateUserCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
        }
    }
}
