using FluentValidation;

namespace Vitastic.App.Features.Wallets.Commands.RevertWalletTransaction
{
    public sealed class RevertWalletTransactionCommandValidation : AbstractValidator<RevertWalletTransactionCommand>
    {
        public RevertWalletTransactionCommandValidation()
        {
            RuleFor(c => c.WalletId).NotEqual(Guid.Empty).WithMessage("شناسه کیف پول نمی تواند خالی باشد.");
            RuleFor(c => c.TransactionId).NotEqual(Guid.Empty).WithMessage("شناسه تراکنش نمی تواند خالی باشد.");
        }
    }
}