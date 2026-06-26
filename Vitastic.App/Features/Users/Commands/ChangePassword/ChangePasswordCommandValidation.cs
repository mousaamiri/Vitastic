using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidation : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("رمز عبور فعلی نمی تواند خالی باشد.")
                .MinimumLength(Password.MinLength).WithMessage($"رمز عبور فعلی باید حداقل {Password.MinLength} کاراکتر باشد.")
                .MaximumLength(Password.MaxLength).WithMessage($"رمز عبور فعلی نمی تواند بیشتر از {Password.MaxLength} کاراکتر باشد.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("رمز عبور جدید نمی تواند خالی باشد.")
                .MinimumLength(Password.MinLength).WithMessage($"رمز عبور جدید باید حداقل {Password.MinLength} کاراکتر باشد.")
                .MaximumLength(Password.MaxLength).WithMessage($"رمز عبور جدید نمی تواند بیشتر از {Password.MaxLength} کاراکتر باشد.");
        }
    }
}