using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.App.Features.Users.Queries.GetByUserName
{
    public sealed class GetUserByUsernameValidation : AbstractValidator<GetUserByUsernameQuery>
    {
        public GetUserByUsernameValidation()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("نام کاربری نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("نام کاربری نمی تواند خالی باشد.")
                .MinimumLength(UserName.MinLength).WithMessage($"نام کاربری باید حداقل {UserName.MinLength} کاراکتر باشد.")
                .MaximumLength(UserName.MaxLength).WithMessage($"نام کاربری باید حداکثر {UserName.MaxLength} کاراکتر باشد.")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("نام کاربری فقط می تواند شامل حروف، اعداد و زیرخط باشد.");
        }
    }
}