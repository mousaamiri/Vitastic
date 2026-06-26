using FluentValidation;

namespace Vitastic.App.Features.Wallets.Commands.CompleteWalletTransaction
{
    public sealed class CompleteWalletTransactionCommandValidation : AbstractValidator<CompleteWalletTransactionCommand>
    {
        public CompleteWalletTransactionCommandValidation()
        {
            RuleFor(x => x.WalletId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کیف پول نمی‌تواند خالی باشد.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("شناسه کیف پول نامعتبر است.");
            RuleFor(x => x.TransactionId)
                .NotEqual(Guid.Empty).WithMessage("شناسه تراکنش نمی‌تواند خالی باشد.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("شناسه تراکنش نامعتبر است.");
        }

    }
}
