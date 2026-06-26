using FluentValidation;

namespace Vitastic.App.Features.Discounts.Queries.CheckValidity
{
    public sealed class CheckDiscountValidityQueryValidation: AbstractValidator<CheckDiscountValidityQuery>
    {
        public CheckDiscountValidityQueryValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
        }
    }
}