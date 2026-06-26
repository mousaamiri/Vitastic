using FluentValidation;
using Vitastic.App.Features.Roles.Queries.CheckRolePermission;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Roles.Queries.CheckRolePermissionByPermissionCode
{
    public sealed class CheckRolePermissionByPermissionCodeQueryValidation : AbstractValidator<CheckRolePermissionByPermissionCodeQuery>
    {
        public CheckRolePermissionByPermissionCodeQueryValidation()
        {
            RuleFor(x => x.RoleId).NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.");
            RuleFor(x => x.PermissionCode)
                .NotEmpty().WithMessage("کد مجوز نباید خالی باشد.")
                .MaximumLength(Permission.MaxCodeLength).WithMessage($"کد دسترسی نمی تواند بیشتر از {Permission.MaxCodeLength} کاراکتر باشد.")
                .MinimumLength(Permission.MinCodeLength).WithMessage($"کد دسترسی نمی تواند کمتر از {Permission.MinCodeLength} کاراکتر باشد.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("کد دسترسی فقط می تواند شامل حروف، اعداد و زیرخط باشد.");

        }
    }
}
