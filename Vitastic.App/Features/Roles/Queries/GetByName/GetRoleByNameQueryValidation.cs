using FluentValidation;

namespace Vitastic.App.Features.Roles.Queries.GetByName
{
    public sealed class GetRoleByNameQueryValidation : AbstractValidator<GetRoleByNameQuery>
    {
        public GetRoleByNameQueryValidation()
        {
            RuleFor(x => x.RoleName).NotEmpty().WithMessage("نام نقش نمی تواند خالی باشد.");
        }
    }
}