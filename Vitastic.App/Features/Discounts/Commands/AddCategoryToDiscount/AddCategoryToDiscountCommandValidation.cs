using FluentValidation;

namespace Vitastic.App.Features.Discounts.Commands.AddCategoryToDiscount
{
    public sealed class AddCategoryToDiscountCommandValidation : AbstractValidator<AddCategoryToDiscountCommand>
    {
        public AddCategoryToDiscountCommandValidation()
        {
            RuleFor(x => x.DiscountId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
            RuleFor(x => x.CategoryId)
                .NotEqual(Guid.Empty).WithMessage("شناسه دسته‌بندی نمی‌تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دسته‌بندی نمی‌تواند خالی باشد.");
        }
    }
}