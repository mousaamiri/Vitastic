using FluentValidation;

namespace Vitastic.App.Features.Roles.Queries.PermissionsList
{
    public sealed class ListRolePermissionsQueryValidation : AbstractValidator<ListRolePermissionsQuery>
    {
        public ListRolePermissionsQueryValidation()
        {
            RuleFor(x => x.RoleId)
                .NotEqual(Guid.Empty).WithMessage("شناسه نقش نامعتبر است.")
                .NotEmpty().WithMessage("شناسه نقش نمیتواند خالی باشد.");
        }
    }
}
