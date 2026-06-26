using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.SetMinimumAmount
{
    public sealed class SetMinimumOrderAmountCommandValidation : AbstractValidator<SetMinimumOrderAmountCommand>
    {
        public SetMinimumOrderAmountCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("حداقل مبلغ سفارش باید بیشتر از ۰ باشد.");
            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("واحد پولی نمی‌تواند خالی باشد.");
        }
    }
}