using FluentValidation;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Discounts.Commands.UpdateDiscount;

public record UpdateDiscountCommand(
    Guid Id,
    string Title,
    string Code,
    decimal? Amount,
    decimal? Percentage,
    decimal? MaxDiscountAmount,
    DateTime StartDate,
    DateTime EndDate,
    int? MaxUsageCount,
    int? MaxUsagePerUser,
    decimal? MinimumOrderAmount
) : ICommand;

public sealed class UpdateDiscountCommandValidator : AbstractValidator<UpdateDiscountCommand>
{
    public UpdateDiscountCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("کد تخفیف نمی تواند خالی باشد.")
            .NotNull().WithMessage("کد تخفیف نمی‌تواند خالی باشد.")
            .MaximumLength(DiscountCode.MaxLength)
            .WithMessage($"کد تخفیف باید حداکثر {DiscountCode.MaxLength} کاراکتر باشد.")
            .MinimumLength(DiscountCode.MinLength)
            .WithMessage($"کد تخفیف باید حداقل {DiscountCode.MinLength} کاراکتر باشد.")
            .Matches(DiscountCode.Pattern).WithMessage("کد تخفیف باید فقط شامل حروف بزرگ و خط فاصله و اعداد باشد.");
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("عنوان تخفیف نمی تواند خالی باشد.")
            .MaximumLength(Title.MaxLength)
            .WithMessage($"عنوان تخفیف نمی تواند بیشتر از {Title.MaxLength} کاراکتر باشد.")
            .MinimumLength(Title.MinLength)
            .WithMessage($"عنوان تخفیف نمی تواند کمتر از {Title.MinLength} کاراکتر باشد.")
            .Matches(Title.TitleRegex)
            .WithMessage(
                "عنوان فقط می‌تواند شامل حروف فارسی/انگلیسی، اعداد، فاصله، ویرگول، نقطه، خط تیره، علامت سوال و ... باشد (بدون کاراکترهای عجیب).");
        // منطق اصلی: یا Amount یا Percentage باید مقدار داشته باشد، نه هر دو
        RuleFor(x => x)
            .Must(x => (x.Amount.HasValue && !x.Percentage.HasValue) ||
                       (!x.Amount.HasValue && x.Percentage.HasValue))
            .WithMessage("باید یکی از مقدار ثابت یا درصد تخفیف را وارد کنید، نه هر دو");

        // اعتبارسنجی Amount
        When(x => x.Amount.HasValue, () =>
        {
            RuleFor(x => x.Amount!.Value)
                .GreaterThan(0).WithMessage("مقدار تخفیف باید بیشتر از صفر باشد")
                .LessThanOrEqualTo(100_000_000).WithMessage("مقدار تخفیف نامعتبر است");
        });

        // اعتبارسنجی Percentage
        When(x => x.Percentage.HasValue, () =>
        {
            RuleFor(x => x.Percentage!.Value)
                .GreaterThan(0).WithMessage("درصد تخفیف باید بیشتر از صفر باشد")
                .LessThanOrEqualTo(100).WithMessage("درصد تخفیف نمی‌تواند بیشتر از 100 باشد");

            // حداکثر مقدار تخفیف فقط برای تخفیف درصدی
            RuleFor(x => x.MaxDiscountAmount)
                .GreaterThan(0).WithMessage("حداکثر مقدار تخفیف باید بیشتر از صفر باشد")
                .When(x => x.MaxDiscountAmount.HasValue);
        });

        // اگر تخفیف مبلغی است، MaxDiscountAmount نباید مقدار داشته باشد
        When(x => x.Amount.HasValue, () =>
        {
            RuleFor(x => x.MaxDiscountAmount)
                .Null().WithMessage("حداکثر مقدار تخفیف فقط برای تخفیف درصدی قابل تعریف است");
        });

        // تاریخ شروع و پایان
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("تاریخ شروع الزامی است")
            .Must(date => date > DateTime.MinValue).WithMessage("تاریخ شروع نامعتبر است");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("تاریخ پایان الزامی است")
            .Must(date => date > DateTime.MinValue).WithMessage("تاریخ پایان نامعتبر است")
            .GreaterThan(x => x.StartDate).WithMessage("تاریخ پایان باید بعد از تاریخ شروع باشد");

        // حداکثر تعداد استفاده کل
        When(x => x.MaxUsageCount.HasValue, () =>
        {
            RuleFor(x => x.MaxUsageCount!.Value)
                .GreaterThan(0).WithMessage("حداکثر تعداد استفاده باید بیشتر از صفر باشد");
        });

        // حداکثر تعداد استفاده هر کاربر
        When(x => x.MaxUsagePerUser.HasValue, () =>
        {
            RuleFor(x => x.MaxUsagePerUser!.Value)
                .GreaterThan(0).WithMessage("حداکثر تعداد استفاده هر کاربر باید بیشتر از صفر باشد");

            // MaxUsagePerUser نباید بیشتر از MaxUsageCount باشد
            RuleFor(x => x)
                .Must(x => !x.MaxUsageCount.HasValue || x.MaxUsagePerUser!.Value <= x.MaxUsageCount.Value)
                .WithMessage("حداکثر استفاده هر کاربر نمی‌تواند بیشتر از حداکثر استفاده کل باشد")
                .When(x => x.MaxUsageCount.HasValue);
        });

        // حداقل مبلغ سفارش
        When(x => x.MinimumOrderAmount.HasValue, () =>
        {
            RuleFor(x => x.MinimumOrderAmount!.Value)
                .GreaterThan(0).WithMessage("حداقل مبلغ سفارش باید بیشتر از صفر باشد");

            // اگر تخفیف مبلغی است، باید کمتر از حداقل مبلغ سفارش باشد
            When(x => x.Amount.HasValue, () =>
            {
                RuleFor(x => x)
                    .Must(x => x.Amount!.Value < x.MinimumOrderAmount!.Value)
                    .WithMessage("مقدار تخفیف نمی‌تواند بیشتر یا مساوی حداقل مبلغ سفارش باشد");
            });
        });
    }
}
public sealed class UpdateDiscountCommandHandler(IDiscountRepository discountRepository) : ICommandHandler<UpdateDiscountCommand>
{
    public async Task<Result> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var startDateUtc = request.StartDate.ToUniversalTime();
        var endDateUtc = request.EndDate.ToUniversalTime();
        var discountId =  DiscountId.CreateFrom(request.Id);
        if(discountId.IsFailure) return discountId.Error;
        Discount? discount = await discountRepository.FindAsync(discountId.Value, cancellationToken);

        if (discount is null)
            return Error.NotFound("Discount.NotFound", "کد تخفیف یافت نشد");

        var newCodeResult = DiscountCode.Create(request.Code);
        if (newCodeResult.IsFailure)
            return newCodeResult.Error;

        if (!discount.Code.Equals(newCodeResult.Value))
        {
            var existingDiscount = await discountRepository.GetByCodeAsync(newCodeResult.Value, cancellationToken);
            if (existingDiscount != null)
                return Error.Conflict("Discount.DuplicateCode", "کد تخفیف تکراری است");
        }

        var titleResult = Title.Create(request.Title);
        if (titleResult.IsFailure)
            return titleResult.Error;

        Money? minimumOrderAmount = null;
        if (request.MinimumOrderAmount.HasValue)
        {
            var minResult = Money.Create(request.MinimumOrderAmount.Value, "IRR");
            if (minResult.IsFailure)
                return minResult.Error;
            minimumOrderAmount = minResult.Value;
        }

        Money? maximumDiscountAmount = null;
        if (request.MaxDiscountAmount.HasValue && request.Percentage.HasValue)
        {
            var maxResult = Money.Create(request.MaxDiscountAmount.Value, "IRR");
            if (maxResult.IsFailure)
                return maxResult.Error;
            maximumDiscountAmount = maxResult.Value;
        }

        var updateResult = discount.Update(
            newCodeResult.Value,
            titleResult.Value,
            startDateUtc,
            endDateUtc,
            minimumOrderAmount,
            maximumDiscountAmount,
            request.MaxUsageCount
        );

        if (updateResult.IsFailure)
            return updateResult.Error;

        // به‌روزرسانی مقدار یا درصد تخفیف
        if (request.Percentage.HasValue)
        {
            var percentResult = discount.UpdatePercentage(request.Percentage.Value);
            if (percentResult.IsFailure)
                return percentResult.Error;
        }
        else if (request.Amount.HasValue)
        {
            var amountResult = Money.Create(request.Amount.Value, "IRR");
            if (amountResult.IsFailure)
                return amountResult.Error;

            var fixedResult = discount.UpdateFixedAmount(amountResult.Value);
            if (fixedResult.IsFailure)
                return fixedResult.Error;
        }

        return Result.Success();
    }
}
