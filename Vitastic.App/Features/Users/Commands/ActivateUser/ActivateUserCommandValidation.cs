using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandValidation : AbstractValidator<ActivateUserCommand>
    {
        public ActivateUserCommandValidation()
        {
            RuleFor(x => x.ActivationCode)
                .NotEmpty().WithMessage("کد فعال سازی نمی تواند خالی باشد.")
                .Length(ActiveCode.MaxLength).WithMessage($"کد فعال سازی باید دقیقاً {ActiveCode.MaxLength} کاراکتر باشد.");
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل جدید نمی تواند خالی باشد.")
                .MaximumLength(Email.MaxLength).WithMessage($"ایمیل جدید نمی تواند بیشتر از {Email.MaxLength} کاراکتر باشد.")
                .MinimumLength(Email.MinLength).WithMessage($"ایمیل جدید نمی تواند کمتر از {Email.MinLength} کاراکتر باشد.")
                .EmailAddress().WithMessage("ایمیل جدید وارد شده معتبر نیست.");
        }
    }
}
