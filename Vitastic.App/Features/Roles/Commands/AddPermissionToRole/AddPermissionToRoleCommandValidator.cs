using FluentValidation;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.App.Features.Roles.Commands.AddPermissionToRole
{
    public sealed class AddPermissionToRoleCommandValidator : AbstractValidator<AddPermissionToRoleCommand>
    {
        public AddPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleId).NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.");
            RuleFor(x => x.PermissionId)
                .NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.")
                .NotEmpty().WithMessage("شناسه نقش نمیتواند خالی باشد.");
        }
    }
}
