using FluentValidation;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Commands.Create.Fixed
{
    public sealed class CreateFixedAmountDiscountCommandValidation : AbstractValidator<CreateFixedAmountDiscountCommand>
    {
        public CreateFixedAmountDiscountCommandValidation()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("کد تخفیف نمی تواند خالی باشد.")
                .NotNull().WithMessage("کد تخفیف نمی‌تواند خالی باشد.")
                .MaximumLength(DiscountCode.MaxLength).WithMessage($"کد تخفیف باید حداکثر {DiscountCode.MaxLength} کاراکتر باشد.")
                .MinimumLength(DiscountCode.MinLength).WithMessage($"کد تخفیف باید حداقل {DiscountCode.MinLength} کاراکتر باشد.")
                .Matches(DiscountCode.Pattern).WithMessage("کد تخفیف باید فقط شامل حروف بزرگ و خط فاصله و اعداد باشد.");
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("عنوان تخفیف نمی تواند خالی باشد.")
                .MaximumLength(Title.MaxLength).WithMessage($"عنوان تخفیف نمی تواند بیشتر از {Title.MaxLength} کاراکتر باشد.")
                .MinimumLength(Title.MinLength).WithMessage($"عنوان تخفیف نمی تواند کمتر از {Title.MinLength} کاراکتر باشد.")
                .Matches(Title.TitleRegex)
                .WithMessage("عنوان فقط می‌تواند شامل حروف فارسی/انگلیسی، اعداد، فاصله، ویرگول، نقطه، خط تیره، علامت سوال و ... باشد (بدون کاراکترهای عجیب).");
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("مبلغ تخفیف باید بیشتر از ۰ باشد.");
            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("واحد پولی نمی‌تواند خالی باشد.");
            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("تاریخ شروع باید قبل از تاریخ پایان باشد.")
                .GreaterThanOrEqualTo(DateTime.Today.AddDays(-7))
                .WithMessage("تاریخ شروع نمی‌تواند خیلی قدیمی باشد."); ;
        }
    }
}
