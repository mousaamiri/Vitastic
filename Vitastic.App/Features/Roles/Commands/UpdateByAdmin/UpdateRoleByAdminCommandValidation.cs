using FluentValidation;
using Vitastic.App.Features.Roles.Commands.Create;
using Vitastic.Domain.Entities.Roles.ValueObjects;

namespace Vitastic.App.Features.Roles.Commands.UpdateByAdmin
{
    public sealed class UpdateRoleByAdminCommandValidation : AbstractValidator<UpdateRoleByAdminCommand>
    {
        public UpdateRoleByAdminCommandValidation()
        {
            RuleFor(x => x.RoleName)
                .NotEmpty().WithMessage("نام نقش نمی تواند خالی باشد.")
                .MaximumLength(RoleName.MaxLenght)
                .WithMessage($"نام نقش نمی تواند بیشتر از {RoleName.MaxLenght} کاراکتر باشد.")
                .Matches(RoleName.Pattern).WithMessage("نام نقش فقط میتواند شامل حروف انگلیسی بزرگ و کوچک و فاصله و اعداد باشد.");
        }
    }
}
