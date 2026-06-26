using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ChangeEmail
{
    public sealed class ChangeEmailCommandValidation : AbstractValidator<ChangeEmailCommand>
    {
        public ChangeEmailCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("شناسه کاربر نمی تواند خالی باشد.");

            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("ایمیل جدید نمی تواند خالی باشد.")
                .MaximumLength(Email.MaxLength).WithMessage($"ایمیل جدید نمی تواند بیشتر از {Email.MaxLength} کاراکتر باشد.")
                .MinimumLength(Email.MinLength).WithMessage($"ایمیل جدید نمی تواند کمتر از {Email.MinLength} کاراکتر باشد.")
                .EmailAddress().WithMessage("ایمیل جدید وارد شده معتبر نیست.");
        }
    }
}