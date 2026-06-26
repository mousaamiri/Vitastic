using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Queries.Calculate
{
    public sealed class CalculateDiscountAmountQueryValidation : AbstractValidator<CalculateDiscountAmountQuery>
    {
        public CalculateDiscountAmountQueryValidation()
        {
            RuleFor(x => x.DiscountCode)
                .NotEmpty().WithMessage("کد تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.OrderTotal).GreaterThan(0).WithMessage("مبلغ سفارش باید بیشتر از صفر باشد.");
            RuleFor(x => x.Currency).NotEmpty().WithMessage("واحد پول نمی‌تواند خالی باشد.")
                .Matches(Currency.CodePattern).WithMessage("واحد پول باید سه حرف بزرگ باشد (مثلاً USD یا IRR).")
                .Length(3).WithMessage("واحد پول باید سه حرف باشد (مثلاً USD یا IRR).");
        }
    }
}
