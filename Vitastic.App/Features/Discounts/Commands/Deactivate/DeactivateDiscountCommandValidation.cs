using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.Deactivate
{
    public sealed class DeactivateDiscountCommandValidation : AbstractValidator<DeactivateDiscountCommand>
    {
        public DeactivateDiscountCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
        }
    }
}