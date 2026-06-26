using FluentValidation;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Permissions.Commands.Create
{
    public sealed class CreatePermissionCommandValidation: AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidation()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد دسترسی نمی تواند خالی باشد.")
                .MaximumLength(Permission.MaxCodeLength).WithMessage($"کد دسترسی نمی تواند بیشتر از {Permission.MaxCodeLength} کاراکتر باشد.")
                .MinimumLength(Permission.MinCodeLength).WithMessage($"کد دسترسی نمی تواند کمتر از {Permission.MinCodeLength} کاراکتر باشد.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("کد دسترسی فقط می تواند شامل حروف، اعداد و زیرخط باشد.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("توضیحات دسترسی نمی تواند خالی باشد.")
                .MaximumLength(Permission.MaxDescriptionLength).WithMessage($"توضیحات دسترسی نمی تواند بیشتر از {Permission.MaxDescriptionLength} کاراکتر باشد.")
                .MinimumLength(Permission.MinDescriptionLength).WithMessage($"توضیحات دسترسی نمی تواند کمتر از {Permission.MinDescriptionLength} کاراکتر باشد.");
        }
    }
}