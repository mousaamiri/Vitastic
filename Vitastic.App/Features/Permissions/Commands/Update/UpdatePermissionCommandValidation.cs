using FluentValidation;
using Vitastic.Domain.Entities.Roles;

namespace Vitastic.App.Features.Permissions.Commands.Update
{
    public sealed class UpdatePermissionCommandValidation: AbstractValidator<UpdatePermissionCommand>
    {
        public UpdatePermissionCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("شناسه مجوز نباید خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مجوز معتبر نیست.");
            When(x => !string.IsNullOrEmpty(x.Description?.Trim()), () =>
            {
                RuleFor(x => x.Code)
                    .MaximumLength(Permission.MaxCodeLength).WithMessage($"کد دسترسی نمی تواند بیشتر از {Permission.MaxCodeLength} کاراکتر باشد.")
                    .MinimumLength(Permission.MinCodeLength).WithMessage($"کد دسترسی نمی تواند کمتر از {Permission.MinCodeLength} کاراکتر باشد.")
                    .Matches("^[a-zA-Z0-9_]+$").WithMessage("کد دسترسی فقط می تواند شامل حروف، اعداد و زیرخط باشد.");
            });
            When(x => !string.IsNullOrEmpty(x.Code?.Trim()), () =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(Permission.MaxDescriptionLength)
                    .WithMessage($"توضیحات دسترسی نمی تواند بیشتر از {Permission.MaxDescriptionLength} کاراکتر باشد.")
                    .MinimumLength(Permission.MinDescriptionLength)
                    .WithMessage($"توضیحات دسترسی نمی تواند کمتر از {Permission.MinDescriptionLength} کاراکتر باشد.");
            });
        }
    }
}