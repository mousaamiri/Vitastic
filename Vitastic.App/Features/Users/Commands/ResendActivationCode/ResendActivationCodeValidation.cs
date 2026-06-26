using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.ResendActivationCode
{
    public sealed class ResendActivationCodeValidation : AbstractValidator<ResendActivationCodeCommand>
    {
        public ResendActivationCodeValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل جدید نمی تواند خالی باشد.")
                .MaximumLength(Email.MaxLength).WithMessage($"ایمیل جدید نمی تواند بیشتر از {Email.MaxLength} کاراکتر باشد.")
                .MinimumLength(Email.MinLength).WithMessage($"ایمیل جدید نمی تواند کمتر از {Email.MinLength} کاراکتر باشد.")
                .EmailAddress().WithMessage("ایمیل جدید وارد شده معتبر نیست.");
        }
    }
}