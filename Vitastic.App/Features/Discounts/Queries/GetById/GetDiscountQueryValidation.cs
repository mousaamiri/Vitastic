using FluentValidation;

namespace Vitastic.App.Features.Discounts.Queries.GetById
{
    public sealed class GetDiscountQueryValidation : AbstractValidator<GetDiscountQuery>
    {
        public GetDiscountQueryValidation()
        {
            RuleFor(x => x.DiscountId).NotEmpty().WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه تخفیف نمی‌تواند خالی باشد.");
        }
    }
}