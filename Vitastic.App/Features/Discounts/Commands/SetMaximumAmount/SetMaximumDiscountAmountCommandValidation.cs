using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.SetMaximumAmount
{
    public sealed class SetMaximumDiscountAmountCommandValidation : AbstractValidator<SetMaximumDiscountAmountCommand>
    {
        public SetMaximumDiscountAmountCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.MaxAmount)
                .GreaterThan(0)
                .WithMessage("حداکثر مبلغ تخفیف باید بیشتر از ۰ باشد.");
            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("واحد پولی نمی‌تواند خالی باشد.");
        }
    }
}