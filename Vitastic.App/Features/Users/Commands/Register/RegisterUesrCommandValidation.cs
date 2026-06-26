using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.Register
{
    public sealed class RegisterUesrCommandValidation : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUesrCommandValidation()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("نام کاربری نمی تواند خالی باشد.")
                .MaximumLength(UserName.MaxLength).WithMessage($"نام کاربری نمی تواند بیشتر از {UserName.MaxLength} کاراکتر باشد.")
                .MinimumLength(UserName.MinLength).WithMessage($"نام کاربری نمی تواند کمتر از {UserName.MinLength} کاراکتر باشد.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل نمی تواند خالی باشد.")
                .MaximumLength(Email.MaxLength).WithMessage($"ایمیل نمی تواند بیشتر از {Email.MaxLength} کاراکتر باشد.")
                .MinimumLength(Email.MinLength).WithMessage($"ایمیل نمی تواند کمتر از {Email.MinLength} کاراکتر باشد.")
                .EmailAddress().WithMessage("ایمیل وارد شده معتبر نیست.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("رمز عبور نمی تواند خالی باشد.")
                .MinimumLength(Password.MinLength).WithMessage($"رمز عبور باید حداقل {Password.MinLength} کاراکتر باشد.")
                .MaximumLength(Password.MaxLength).WithMessage($"رمز عبور نمی تواند بیشتر از {Password.MaxLength} کاراکتر باشد.");
        }
    }
}