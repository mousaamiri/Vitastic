using FluentValidation;

namespace Vitastic.App.Features.Users.Commands.RemoveRoleFromUser
{
    public sealed class RemoveRoleFromUserCommandValidation : AbstractValidator<RemoveRoleFromUserCommand>
    {
        public RemoveRoleFromUserCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");

            RuleFor(x => x.RoleId)
                .NotEqual(Guid.Empty).WithMessage("شناسه نقش نمی تواند خالی باشد.");
        }
    }
}