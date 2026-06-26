using FluentValidation;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Roles.Commands.RemovePermissionFromRole
{
    public sealed class RemovePermissionFromRoleCommandValidation : AbstractValidator<RemovePermissionFromRoleCommand>
    {
        public RemovePermissionFromRoleCommandValidation()
        {
            RuleFor(x => x.RoleId).NotEmpty().WithMessage("شناسه مجوز نباید خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مجوز معتبر نیست.");
            RuleFor(x => x.PermissionId)
                .NotEmpty().WithMessage("شناسه مجوز نباید خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مجوز معتبر نیست.");
        }
    }
}
