using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Queries.GetByEmail
{
    public sealed class GetUserByPhoneNumberValidation : AbstractValidator<GetUserByEmailQuery>
    {
        public GetUserByPhoneNumberValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("ایمیل نمی تواند خالی باشد.")
                .EmailAddress().WithMessage("ایمیل وارد شده معتبر نیست.")
                .MinimumLength(Email.MinLength).WithMessage($"ایمیل باید حداقل {Email.MinLength} کاراکتر باشد.")
                .MaximumLength(Email.MaxLength).WithMessage($"ایمیل باید حداکثر {Email.MaxLength} کاراکتر باشد.");
        }
    }
}
