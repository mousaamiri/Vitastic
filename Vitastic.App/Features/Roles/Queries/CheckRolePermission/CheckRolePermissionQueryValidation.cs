using FluentValidation;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermission
{
    public sealed class CheckRolePermissionQueryValidation : AbstractValidator<CheckRolePermissionQuery>
    {
        public CheckRolePermissionQueryValidation()
        {
            RuleFor(x => x.RoleId).NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.");
            RuleFor(x => x.PermissionId)
                .NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.")
                .NotEmpty().WithMessage("شناسه نقش نمیتواند خالی باشد.");
        }
    }
}
