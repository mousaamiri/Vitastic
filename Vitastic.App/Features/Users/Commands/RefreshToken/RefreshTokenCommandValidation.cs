using FluentValidation;

namespace Vitastic.App.Features.Users.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandValidation
        : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidation()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh Token نمی‌تواند خالی باشد.");
        }
    }
}