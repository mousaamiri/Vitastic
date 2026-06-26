using FluentValidation;

namespace Vitastic.App.Features.Roles.Queries.GetById
{
    public sealed class GetRoleQueryValidation : AbstractValidator<GetRoleQuery>
    {
        public GetRoleQueryValidation()
        {
            RuleFor(x => x.RoleId).NotEqual(Guid.Empty).WithMessage("شناسه نقش معتبر نمی باشد.");
        }
    }
}