using FluentValidation;

namespace Vitastic.App.Features.Carts.Commands.CheckoutCart
{
    public sealed class CheckoutCartCommandValidator : AbstractValidator<CheckoutCartCommand>
    {
        public CheckoutCartCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("شناسه کاربر الزامی است");
        }
    }
}