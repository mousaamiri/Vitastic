using FluentValidation;

namespace Vitastic.App.Features.Users.Queries.CheckUserRole
{
    public sealed class CheckUserRoleValidation : AbstractValidator<CheckUserRoleQuery>
    {
        public CheckUserRoleValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
            RuleFor(x => x.RoleId).NotEqual(Guid.Empty).WithMessage("شناسه نقش نمی تواند خالی باشد.");
        }
    }
}