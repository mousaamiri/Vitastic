using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.Extend
{
    public sealed class ExtendDiscountEndDateCommandValidation : AbstractValidator<ExtendDiscountEndDateCommand>
    {
        public ExtendDiscountEndDateCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.NewEndDate)
                .GreaterThan(DateTime.Now).WithMessage("تاریخ پایان جدید باید بعد از تاریخ فعلی باشد.");
        }
    }
}