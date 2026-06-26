using FluentValidation;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Payments.Commands.Create
{
    public sealed class CreatePaymentTransactionCommandValidation : AbstractValidator<CreatePaymentTransactionCommand>
    {
        public CreatePaymentTransactionCommandValidation()
        {
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("مبلغ تراکنش باید بزرگتر از صفر باشد.");
            RuleFor(x => x.WalletId).NotEqual(Guid.Empty).When(x => x.WalletId.HasValue)
                .WithMessage("شناسه کیف پول معتبر نمی باشد.");
            When(x => !string.IsNullOrEmpty(x.Description),
                () =>
                {
                    RuleFor(x => x.Description).MaximumLength(Description.MaxLength)
                        .WithMessage($"توضیحات نمی تواند بیشتر از {Description.MaxLength} کاراکتر باشد.");
                });
        }
    }
}