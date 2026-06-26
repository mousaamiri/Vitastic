using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ResetPassword
{
    public sealed class ResetPasswordValidation : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordValidation()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("کد فعال سازی نمی تواند خالی باشد.")
                .Length(ActiveCode.MaxLength).WithMessage($"کد فعال سازی باید دقیقاً {ActiveCode.MaxLength} کاراکتر باشد.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("رمز عبور جدید نمی تواند خالی باشد.")
                .MinimumLength(Password.MinLength).WithMessage($"رمز عبور جدید باید حداقل {Password.MinLength} کاراکتر باشد.")
                .MaximumLength(Password.MaxLength).WithMessage($"رمز عبور جدید نمی تواند بیشتر از {Password.MaxLength} کاراکتر باشد.");
        }
    }
}
