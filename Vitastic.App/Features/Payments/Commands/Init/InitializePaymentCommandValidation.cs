using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Payments.Commands.Init
{
    public sealed class InitializePaymentCommandValidation : AbstractValidator<InitializePaymentCommand>
    {
        public InitializePaymentCommandValidation()
        {
            RuleFor(x => x)
                .Must(x => x.WalletId is not null || x.OrderId is not null)
                .WithMessage("هر تراکنش باید متعلق به یک کیف پول یا سفارش باشد. باید حداقل یکی از این دو ست شده باشند.")
                .WithErrorCode("Payment.MissingTarget");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage(" مبلغ باید بیشتر از صفر باشد.   ");
            When(x => x.WalletId.HasValue, () =>
            {
                RuleFor(x => x.WalletId)
                    .NotEqual(Guid.Empty).WithMessage("شناسه کیف پول نمی تواند خالی باشد.");
            });
            When(x => x.OrderId.HasValue, () =>
            {
                RuleFor(x => x.OrderId).NotEqual(Guid.Empty).WithMessage("شناسه سفارش نمی تواند خالی باشد.");
            });
            When(x => !string.IsNullOrEmpty(x.CallbackUrl), () =>
            {
                RuleFor(x => x.CallbackUrl).Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                    .WithMessage("آدرس بازگشتی نامعتبر است.");
            });
            When(x => !string.IsNullOrEmpty(x.Description), () =>
            {
                RuleFor(x => x.Description).MaximumLength(Description.MaxLength)
                    .WithMessage($"توضیحات نمی تواند بیشتر از {Description.MaxLength} کاراکتر باشد.");
            });
        }
    }
}
