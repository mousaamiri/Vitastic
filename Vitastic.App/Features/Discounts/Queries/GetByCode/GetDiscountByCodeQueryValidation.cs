using FluentValidation;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Queries.GetByCode
{
    public sealed class GetDiscountByCodeQueryValidation : AbstractValidator<GetDiscountByCodeQuery>
    {
        public GetDiscountByCodeQueryValidation()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد تخفیف نمی‌تواند خالی باشد.")
                .NotNull().WithMessage("کد تخفیف نمی‌تواند خالی باشد.")
                .MaximumLength(DiscountCode.MaxLength).WithMessage($"کد تخفیف باید حداکثر {DiscountCode.MaxLength} کاراکتر باشد.")
                .MinimumLength(DiscountCode.MinLength).WithMessage($"کد تخفیف باید حداقل {DiscountCode.MinLength} کاراکتر باشد.")
                .Matches(DiscountCode.Pattern).WithMessage($"کد تخفیف فقط می‌تواند شامل حروف انگلیسی، اعداد و خط تیره باشد")
                ;
        }
    }
}
