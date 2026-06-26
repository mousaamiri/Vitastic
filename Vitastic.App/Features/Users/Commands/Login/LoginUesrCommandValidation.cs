using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.Login
{
    public sealed class LoginUserCommandValidation : AbstractValidator<LoginUserCommand>
    {
        public LoginUserCommandValidation()
        {
            RuleFor(x => x.Identifier)
                .NotEmpty().WithMessage("نام کاربری یا ایمیل نمی‌تواند خالی باشد.")
                .MaximumLength(320).WithMessage("مقدار واردشده بیش از حد طولانی است.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور نمی‌تواند خالی باشد.")
                .MinimumLength(Password.MinLength)
                .WithMessage($"رمز عبور باید حداقل {Password.MinLength} کاراکتر باشد.")
                .MaximumLength(Password.MaxLength)
                .WithMessage($"رمز عبور نمی‌تواند بیشتر از {Password.MaxLength} کاراکتر باشد.");
        }
    }
}
