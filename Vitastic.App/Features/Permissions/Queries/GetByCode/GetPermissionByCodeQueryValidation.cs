using FluentValidation;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Permissions.Queries.GetByCode
{
    public sealed class GetPermissionByCodeQueryValidation : AbstractValidator<GetPermissionByCodeQuery>
    {
        public GetPermissionByCodeQueryValidation()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد مجوز نباید خالی باشد.")
                .MaximumLength(Permission.MaxCodeLength).WithMessage($"کد دسترسی نمی تواند بیشتر از {Permission.MaxCodeLength} کاراکتر باشد.")
                .MinimumLength(Permission.MinCodeLength).WithMessage($"کد دسترسی نمی تواند کمتر از {Permission.MinCodeLength} کاراکتر باشد.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("کد دسترسی فقط می تواند شامل حروف، اعداد و زیرخط باشد.");

        }
    }
}